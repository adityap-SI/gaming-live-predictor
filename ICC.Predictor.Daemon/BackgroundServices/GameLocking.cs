using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ICC.Predictor.Blanket.Notification;
using ICC.Predictor.Contracts.BackgroundServices;
using ICC.Predictor.Blanket.Scoring;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;
using Topics = ICC.Predictor.Blanket.Notification.Topics;

namespace ICC.Predictor.Daemon.BackgroundServices
{
    class GameLocking : BaseService<GameLocking>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private string _Service = nameof(GameLocking);
        private Blanket.BackgroundServices.GameLocking _Locking;
        private Blanket.Feeds.Ingestion _Ingestion;
        private Topics _NotificationTopics;
        private Publish _NotificationPublish;
        private readonly Blanket.BackgroundServices.MatchAnswerCalculation _MatchAnswerCalculationContext;
        private readonly Process _ScoringContext;

        //  private Blanket.Stats.Player _Stats;
        private int _Interval;
        private int _MatchLockMinutes;
        private double _LockFirstInningAfter;
        private double _LockSecondInningAfter;
        private int _MatchLockNotificationMinutesBefore;
        private int _SubmitLineupsMinutesBefore;
        private int _NotificationsDelaySeconds;

        public GameLocking(ILogger<GameLocking> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _MatchLockMinutes = serviceSettings.Value.GameLocking.MatchLockMinutes;
            _Interval = serviceSettings.Value.GameLocking.IntervalSeconds;
            _LockFirstInningAfter = serviceSettings.Value.GameLocking.LockFirstInningAfter;
            _LockSecondInningAfter = serviceSettings.Value.GameLocking.LockSecondInningAfter;
            _MatchLockNotificationMinutesBefore = serviceSettings.Value.GameLocking.MatchLockNotificationMinutesBefore;
            _SubmitLineupsMinutesBefore = serviceSettings.Value.GameLocking.SubmitLineupsMinutesBefore;
            _NotificationsDelaySeconds = serviceSettings.Value.NotificationDelaySeconds;
            _Locking = new Blanket.BackgroundServices.GameLocking(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationTopics = new Blanket.Notification.Topics(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublish = new Publish(appSettings, aws, postgre, redis, cookies, asset);
            _MatchAnswerCalculationContext = new Blanket.BackgroundServices.MatchAnswerCalculation(appSettings, aws, postgre, redis, cookies, asset);
            _ScoringContext = new Process(appSettings, aws, postgre, redis, cookies, asset);
            //_Stats = new Blanket.Stats.Player(appSettings, aws, postgre, redis, cookies, asset);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Catcher("started.");

            //Timer runs immediately. Periodic intervals is disabled.
            _Timer = new Timer(Process, null, 0, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private void Process(object state)
        {
            Run(state);

            //Timer runs after the interval period. Periodic intervals is disabled.
            _Timer?.Change(Convert.ToInt32(TimeSpan.FromSeconds(_Interval).TotalMilliseconds), Timeout.Infinite);
        }

        private async void Run(object state)
        {
            try
            {
                bool success = false;
                // List<Fixtures> nextFixtures = _Locking.NextMatchList();
                int retval;

                await CheckLineupsTossProcess();
                await CheckGameLocking();
                // await CheckInningLocking();
            }
            catch (Exception ex)
            {
                Catcher("Run", LogLevel.Error, ex);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);

            Catcher($"{_Service} stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }

        private async Task<bool> SendPushNotification(int EventType, int? MatchId, string TeamA = "", string TeamB = "")
        {
            NotificationText notificationText = new NotificationText();
            notificationText = await _NotificationTopics.GetNotificationText();
            string message = "";
            bool success = false;
            switch (EventType)
            {
                // Notification Before Match Lock
                case 1:
                    message = notificationText.preMatch;
                    List<NotificationMessages> n = new List<NotificationMessages>();
                    n.Add(new NotificationMessages
                    {
                        EventId = (int)NotificationEvents.Generic,
                        Language = "en",
                        Message = string.Format(message, TeamA.Trim(), TeamB.Trim()),
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Notification 1st Inning Unlock
                case 2:
                    message = notificationText.openInningOne;
                    n = new List<NotificationMessages>();
                    n.Add(new NotificationMessages
                    {
                        EventId = (int)NotificationEvents.Generic,
                        Language = "en",
                        Message = string.Format(message, TeamA.Trim(), TeamB.Trim()),
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Notification 2nd Inning Unlock
                case 3:
                    message = notificationText.openInningTwo;
                    n = new List<NotificationMessages>();
                    n.Add(new NotificationMessages
                    {
                        EventId = (int)NotificationEvents.Generic,
                        Language = "en",
                        Message = string.Format(message, TeamA.Trim(), TeamB.Trim()),
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Point Calculation
                case 4:
                    message = notificationText.preMatch;
                    n = new List<NotificationMessages>();
                    n.Add(new NotificationMessages
                    {
                        EventId = (int)NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n);
                    break;
            }
            return success;
        }

        #region " Helper Functions  "

        public async Task<bool> CheckLineupsTossProcess()
        {
            bool success = false;
            List<Fixtures> nextFixtures = _Locking.NextMatchList();
            int retval;

            // Check For Match LineUps And Toss Process
            foreach (Fixtures mFixture in nextFixtures)
            {
                MatchFeed mMatchFeed = new MatchFeed();
                List<Lineups> mLineups = new List<Lineups>();
                try
                {
                    mMatchFeed = _Locking.GetMatchScoresFeed(mFixture.Matchfile);
                    if (IsFinalLinups(mMatchFeed))
                        mLineups = _Locking.GetLineupsFromMatchFeed(mMatchFeed);
                }
                catch (Exception ex) { }

                if (DateTime.Compare(Library.Utility.TimeZone.UTCtoIST(DateTime.UtcNow),
                            GenericFunctions.ToUSCulture(mFixture.Deadlinedate).AddMinutes(_SubmitLineupsMinutesBefore)) >= 0)
                {
                    if (mLineups != null && mLineups.Count() > 0 && mFixture.Is_Lineup_Process == 0)
                    {
                        Catcher("Lineups submission started. MatchId:" + mFixture.MatchId);
                        success = await InsertMatchLineups(mFixture.MatchId, mLineups);
                        Catcher("Lineups submission Completed. MatchId:" + mFixture.MatchId + "Result" + success);
                        if (success)
                        {
                            retval = await IngestThis(mFixture.MatchId);
                        }

                    }
                }
                else
                    Catcher("LineUps Process is scheduled to run at: " + Convert.ToDateTime(mFixture.Deadlinedate).AddMinutes(_SubmitLineupsMinutesBefore)
                        + Environment.NewLine + "Current Time IST: " + Library.Utility.TimeZone.UTCtoIST(DateTime.UtcNow));

                if (mFixture.Is_Lineup_Process == 1 && mFixture.Is_Toss_Process == 0)
                {
                    //if(mMatchFeed.Matchdetail.Tosswonby != null && mMatchFeed.Innings.Count() > 0)
                    if (mMatchFeed.Matchdetail.Tosswonby != null)
                    {
                        Innings mInning = mMatchFeed.Innings.Where(s => s.Number.ToLower() == "first").FirstOrDefault();
                        if (mInning != null && mInning.Battingteam != null)
                        {
                            int inninOneBatTeamId = mInning.Battingteam.SmartIntParse();
                            int inninOneBowlTeamId = inninOneBatTeamId == mFixture.TeamAId ? mFixture.TeamBId : mFixture.TeamAId;
                            Catcher("Toss submission Started. MatchId:" + mFixture.MatchId);
                            success = await ProcessMatchToss(mFixture.MatchId, inninOneBatTeamId, inninOneBowlTeamId, inninOneBowlTeamId, inninOneBatTeamId);
                            Catcher("Toss submission Completed. MatchId:" + mFixture.MatchId + "Result" + success);
                            if (success)
                            {
                                retval = await IngestThis(mFixture.MatchId);
                            }
                        }
                    }
                }
            }

            return success;
        }

        public async Task<bool> CheckGameLocking()
        {
            bool success = false;
            List<Fixtures> nextFixtures = _Locking.NextMatchList();
            int retval;

            try
            {
                if (nextFixtures != null && nextFixtures.Any())
                {
                    foreach (Fixtures fix in nextFixtures)
                    {
                        if (fix.MatchStatus == 1 && fix.Match_Inning_Status == 0 && fix.Is_Lineup_Process == 1 && fix.Is_Toss_Process == 1)
                        {
                            LockList lockList = new LockList()
                            {
                                MatchIdList = nextFixtures.Select(o => o.MatchId).ToList()
                                //MatchdayIdList = fixtures.Select(o => o.Ma).ToList()
                            };

                            Catcher(GenericFunctions.Serialize(lockList));

                            if (DateTime.Compare(Library.Utility.TimeZone.UTCtoIST(DateTime.UtcNow),
                                GenericFunctions.ToUSCulture(fix.Deadlinedate).AddMinutes(_MatchLockNotificationMinutesBefore)) >= 0)
                            {
                                bool notificationStatus = await _NotificationTopics.GetMatchNotificationStatus(fix.MatchId);

                                if (notificationStatus == false)
                                {
                                    await SendPushNotification(1, fix.MatchId, fix.TeamAName, fix.TeamBName);
                                    await _NotificationTopics.UpdateMatchNotificationStatus(fix.MatchId);
                                }
                            }
                            if (DateTime.Compare(Library.Utility.TimeZone.UTCtoIST(DateTime.UtcNow),
                                GenericFunctions.ToUSCulture(fix.Deadlinedate).AddMinutes(_MatchLockMinutes)) >= 0)
                            {
                                Catcher("Game Locking initiated.");
                                //Game Locking call
                                success = await RunGameLocking(lockList);
                                if (success)
                                {
                                    foreach (int mLockMatch in lockList.MatchIdList)
                                    {
                                        retval = await IngestThis(mLockMatch);
                                    }
                                }
                            }
                            else
                                Catcher("Game Locking is scheduled to run at: " + Convert.ToDateTime(fix.Deadlinedate).AddMinutes(_MatchLockMinutes)
                                    + Environment.NewLine + "Current Time IST: " + Library.Utility.TimeZone.UTCtoIST(DateTime.UtcNow));
                        }

                    }
                    //Fixtures fix = nextFixtures[0];
                }
                else
                    Catcher("Fixtures IsCurrent and IsLocked value is unexpected.");
            }
            catch (Exception ex)
            {
                Catcher("Run", LogLevel.Error, ex);
            }

            return success;
        }

        public async Task<bool> CheckInningLocking()
        {
            bool success = false;
            int retval;
            try
            {
                List<Fixtures> liveFixtures = _Locking.LiveMatchList();

                foreach (Fixtures mFixtures in liveFixtures)
                {
                    if (mFixtures.Is_Lineup_Process == 1 && mFixtures.Is_Toss_Process == 1 && mFixtures.MatchStatus == 2)
                    {
                        MatchFeed mMatchFeed = _Locking.GetMatchScoresFeed(mFixtures.Matchfile.ToString());
                        if (mFixtures.Match_Inning_Status == 1)
                        {
                            if (mMatchFeed.Innings != null && mMatchFeed.Innings.Any())
                            {
                                Innings Firstinning = new Innings();
                                try
                                {
                                    Firstinning = mMatchFeed.Innings.Where(c => c.Number.ToLower() == "first").FirstOrDefault();
                                }
                                catch (Exception ex) { Firstinning = null; }

                                if (Firstinning != null)
                                {
                                    ///if (Convert.ToDouble(Firstinning.Overs) >= _LockFirstInningAfter)
                                    //{
                                    success = await OpenInning(mFixtures.MatchId, 1);
                                    if (success)
                                    {
                                        retval = await IngestThis(mFixtures.MatchId);
                                        Catcher("Service Sleeping");
                                        Thread.Sleep(_NotificationsDelaySeconds);
                                        Catcher("Sending Notification For Inning One Unlock");
                                        await SendPushNotification(2, mFixtures.MatchId, mFixtures.TeamAName, mFixtures.TeamBName);
                                        //  }
                                        Catcher("Inning Open For Match = " + mFixtures.MatchId + " Inning No = " + 1 + " Result = " + success);
                                    }
                                }
                            }
                        }
                        else if (mFixtures.Match_Inning_Status == 2)
                        {
                            if (mMatchFeed.Innings != null && mMatchFeed.Innings.Any())
                            {
                                Innings Firstinning = new Innings();
                                try
                                {
                                    Firstinning = mMatchFeed.Innings.Where(c => c.Number.ToLower() == "first").FirstOrDefault();
                                }
                                catch (Exception ex) { Firstinning = null; }

                                if (Firstinning != null)
                                {
                                    if (Convert.ToDouble(Firstinning.Overs) >= _LockFirstInningAfter)
                                    {
                                        success = await LockInning(mFixtures.MatchId, 1);
                                        if (success)
                                        {
                                            retval = await IngestThis(mFixtures.MatchId);
                                        }
                                        Catcher("Inning Locking For Match = " + mFixtures.MatchId + " Inning No = " + 1 + " Result = " + success);
                                    }
                                }
                            }
                        }
                        else if (mFixtures.Match_Inning_Status == 3)
                        {
                            if (mMatchFeed.Innings != null && mMatchFeed.Innings.Any())
                            {
                                Innings Firstinning = new Innings();
                                try
                                {
                                    Firstinning = mMatchFeed.Innings.Where(c => c.Number.ToLower() == "first").FirstOrDefault();
                                }
                                catch (Exception ex) { Firstinning = null; }

                                if (Firstinning != null)
                                {
                                    if (Convert.ToDouble(Firstinning.Overs) == Convert.ToDouble(Firstinning.AllottedOvers) || Convert.ToInt32(Firstinning.Wickets) == 10)
                                    {
                                        success = await OpenInning(mFixtures.MatchId, 2);
                                        if (success)
                                        {
                                            bool firstInningAnswersuccess = await SubmitFirstInningAnswer(mFixtures);
                                            retval = await IngestThis(mFixtures.MatchId);
                                            Catcher("Service Sleeping");
                                            Thread.Sleep(_NotificationsDelaySeconds);
                                            Catcher("Sending Notification For Inning Two Unlock");
                                            await SendPushNotification(3, mFixtures.MatchId, mFixtures.TeamAName, mFixtures.TeamBName);
                                            Catcher("Inning Open For Match = " + mFixtures.MatchId + " Inning No = " + 2 + " Result = " + success);
                                        }
                                    }
                                }
                            }
                        }
                        else if (mFixtures.Match_Inning_Status == 4)
                        {
                            if (mMatchFeed.Innings != null && mMatchFeed.Innings.Any())
                            {
                                Innings SecondInning = new Innings();
                                try
                                {
                                    SecondInning = mMatchFeed.Innings.Where(c => c.Number.ToLower() == "second").FirstOrDefault();
                                }
                                catch (Exception ex) { SecondInning = null; }

                                if (SecondInning != null)
                                {
                                    if (Convert.ToDouble(SecondInning.Overs) >= _LockSecondInningAfter)
                                    {
                                        success = await LockInning(mFixtures.MatchId, 2);
                                        if (success)
                                        {
                                            retval = await IngestThis(mFixtures.MatchId);
                                            Catcher("Inning Locking For Match = " + mFixtures.MatchId + " Inning No = " + 2 + " Result = " + success);
                                        }
                                    }
                                }
                            }
                        }
                        else if (mFixtures.Match_Inning_Status == 5 && mMatchFeed.Matchdetail.Status.ToLower().Trim() == "match ended")
                        {
                            success = await LockInning(mFixtures.MatchId, -1);
                            if (success)
                                retval = await IngestThis(mFixtures.MatchId);
                            Catcher("Match Ended Match Id = " + mFixtures.MatchId + " Result = " + success);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Catcher("Run : Check For Live Matches", LogLevel.Error, ex);
            }

            return success;
        }

        public async Task<bool> RunGameLocking(LockList lockList)
        {
            int lockVal = 0;
            int unlockVal = 0;
            int optType = 1;
            int inningOnMatchLock = 1;
            try
            {
                foreach (int mMatchId in lockList.MatchIdList)
                {
                    lockVal = _Locking.Lock(optType, mMatchId, 0);
                    //Sending notification
                    LockNotify(mMatchId, lockVal);
                    Catcher("ICC Game Locking completed. Match Id = " + mMatchId + " RetVal = " + lockVal);
                    // unlockVal =_Locking.UnLock(optType, mMatchId, inningOnMatchLock);
                    // Catcher("ICC Inning Unlocking completed. Match Id = " + mMatchId + " InningNo = " + inningOnMatchLock  +  " RetVal = " + unlockVal);
                }
            }
            catch (Exception ex) { Catcher("RunGameLocking", LogLevel.Error, ex); }

            return lockVal == 1;
        }

        public async Task<bool> LockInning(int matchId, int inningNo)
        {
            int lockVal = 0, optType = 1;
            try
            {
                lockVal = _Locking.Lock(optType, matchId, inningNo);
                if (inningNo != -1)
                    InningChangeNotify(matchId, lockVal, inningNo, true);
            }
            catch (Exception ex)
            {
                Catcher("LockInning", LogLevel.Error, ex);
            }

            return lockVal == 1;
        }

        public async Task<bool> OpenInning(int matchId, int inningNo)
        {
            int lockVal = 0, optType = 1;
            try
            {
                lockVal = _Locking.UnLock(optType, matchId, inningNo);
                InningChangeNotify(matchId, lockVal, inningNo, false);

            }
            catch (Exception ex) { Catcher("OpenInning", LogLevel.Error, ex); }

            return lockVal == 1;
        }

        public async Task<bool> InsertMatchLineups(int matchId, List<Lineups> lineups)
        {
            int lockVal = 0, optType = 1;
            try
            {
                lockVal = _Locking.InsertMatchLineups(optType, matchId, lineups);
                LineupsTossProcessNotify(matchId, lockVal, true);
            }
            catch (Exception ex) { Catcher("InsertMatchLineups", LogLevel.Error, ex); }

            return lockVal == 1;
        }

        public async Task<bool> ProcessMatchToss(int matchId, int inningOneBatTeamId, int inningOneBowlTeamId, int inningTwoBatTeamId, int inningTwoBowlTeamId)
        {
            int lockVal = 0, optType = 1;
            try
            {
                lockVal = _Locking.ProcessMatchToss(optType, matchId, inningOneBatTeamId, inningOneBowlTeamId, inningTwoBatTeamId, inningTwoBowlTeamId);
                LineupsTossProcessNotify(matchId, lockVal, false);
            }
            catch (Exception ex) { Catcher("ProcessMatchToss", LogLevel.Error, ex); }

            return lockVal == 1;
        }

        public async Task<int> IngestThis(int matchId)
        {
            int retval = await _Ingestion.Fixtures();
            retval = await _Ingestion.MatchInningStatus(matchId);
            retval = await _Ingestion.CurrentGamedayMatches();
            retval = await _Ingestion.Questions(matchId);
            return retval;
        }

        public bool IsFinalLinups(MatchFeed mMatchFeed)
        {
            bool result = false;

            foreach (string mTeamId in mMatchFeed.Teams.Keys)
            {
                if (mMatchFeed.Teams[mTeamId].Players.Count() >= 11)
                    result = true;
                else
                    result = false;

                break;
            }

            return result;
        }

        public async Task<bool> SubmitFirstInningAnswer(Fixtures mMatchFixture)
        {
            #region " FIRST INNING ANSWERS "
            bool success = false;
            //List<Fixtures> mFirstInningFixtures = new List<Fixtures>();
            //mFirstInningFixtures = _MatchAnswerCalculationContext.GetFirstInningFinishedMatches(matchId);

            if (mMatchFixture != null)
            {
                Catcher("Answers Submission Started For Match Id : " + mMatchFixture.MatchId + " Inning No: 1");

                success = _ScoringContext.CalculateFirstInningAnswers(mMatchFixture);

                Catcher("Answers Submission Completed For Match Id : " + mMatchFixture.MatchId + " Inning No: 1" + " Result :" + success);

                if (success)
                {
                    int retval = await _Ingestion.Questions(mMatchFixture.MatchId);
                    Catcher("Match Questions ingestion completed for matchId : " + mMatchFixture.MatchId + " retVal " + retval);
                }
                //Catcher("Fixtures ingestion started.");

                //await _Ingestion.Fixtures();

                //Catcher("Fixtures ingestion completed. CurrentGamedayMatches ingestion started.");

                //await _Ingestion.CurrentGamedayMatches();

                //Catcher("CurrentGamedayMatches ingestion completed. Teams Recent Results igestion started.");

                //await _Ingestion.GetRecentResults();

                //Catcher("Teams Recent Results ingestion completed.");

            }

            return success;
            #endregion " FIRST INNING ANSWERS "
        }

        #endregion

        #region " Notification body "

        private void LockNotify(int MatchId, int lockResult)
        {
            try
            {
                //string caption = "Game Locking [MatchdayId: " + MatchDayId + " MatchId: " + MatchId + "]";
                string caption = "Game Locking [MatchId: " + MatchId + "]";
                string remark = lockResult == 1 ? "SUCCESS" : "FAILED";

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "ICC Predictor - [ Lock RetVal: " + lockResult + " ]<br/>";

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        private void InningChangeNotify(int MatchId, int lockResult, int InningNo, bool IsLock)
        {
            try
            {
                //string caption = "Game Locking [MatchdayId: " + MatchDayId + " MatchId: " + MatchId + "]";
                string caption = "Inning" + (IsLock ? "Locking" : "Open") + "[MatchId: " + MatchId + " Inning: " + InningNo + " ]";
                string remark = lockResult == 1 ? "SUCCESS" : "FAILED";

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "ICC Predictor - [ Lock RetVal: " + lockResult + " ]<br/>";

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        private void LineupsTossProcessNotify(int MatchId, int lockResult, bool IsLineups)
        {
            try
            {
                //string caption = "Game Locking [MatchdayId: " + MatchDayId + " MatchId: " + MatchId + "]";
                string caption = (IsLineups ? "Lineups" : "Toss") + " Process " + "[MatchId: " + MatchId + " ]";
                string remark = lockResult == 1 ? "SUCCESS" : "FAILED";

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "ICC Predictor - [ Lock RetVal: " + lockResult + " ]<br/>";

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        private void LiveNotify(LockList list, int lockResult)
        {
            try
            {
                string caption = "Game Live [MatchdayIds: " + string.Join(",", list.MatchdayIdList) + " MatchIds: " + string.Join(",", list.MatchIdList) + "]";
                string remark = lockResult == 1 ? "SUCCESS" : "FAILED";

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "ICC Predictor - [ Live RetVal: " + lockResult + " ]<br/>";

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion

    }
}
