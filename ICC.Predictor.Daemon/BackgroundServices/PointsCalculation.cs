using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ICC.Predictor.Contracts.Admin;
using System.Data;
using System.Text;
using ICC.Predictor.Blanket.Notification;
using ICC.Predictor.Contracts.BackgroundServices;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Contracts.Automate;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Daemon.BackgroundServices
{
    public class PointsCalculation : BaseService<PointsCalculation>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.BackgroundServices.PointsCalculation _Calculation;
        private readonly Blanket.Feeds.Gameplay _Feeds;
        private Blanket.Feeds.Ingestion _Ingestion;
        private Blanket.Notification.Topics _NotificationTopics;
        private Publish _NotificationPublish;
        private int _Interval;
        private int _NotificationsDelaySeconds;
        private string _LeaderBoardType;

        public PointsCalculation(ILogger<PointsCalculation> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _Calculation = new Blanket.BackgroundServices.PointsCalculation(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Feeds = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationTopics = new Blanket.Notification.Topics(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublish = new Publish(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.PointsCalculation.IntervalMinutes;
            _NotificationsDelaySeconds = serviceSettings.Value.NotificationDelaySeconds;
            _LeaderBoardType = serviceSettings.Value.PointsCalculation.LeaderBoardType;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Catcher($"{_Service} started.");

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

                Matchdays matchdays = new Matchdays();
                matchdays = _Calculation.Matchdays();
                //if (matchdays == null || matchdays.GamedayId == 0 || matchdays.Matchday == 0)
                if (matchdays == null || matchdays.GamedayId == 0)
                    return;

                Catcher("MatchdayId found: " + GenericFunctions.Serialize(matchdays));


                Catcher("Points calculation process started for gameDayId " + matchdays.GamedayId + " matchdayId " + matchdays.Matchday);

                int retVal = _Calculation.UserPointsProcess(matchdays.GamedayId, matchdays.Matchday);

                Catcher("Points calculation process completed. Retval: " + retVal);

                Catcher("Reports process started.");
                DataSet ds = _Calculation.UserPointsProcessReports(retVal, matchdays.GamedayId, matchdays.Matchday);

                StringBuilder rp = _Calculation.ParseReports(ds);

                Catcher("Reports process completed.");

                if (retVal == 1)
                {

                    Catcher("Match Questions ingestion started");

                    List<Fixtures> fixtures = new List<Fixtures>();
                    HTTPResponse httpResponse = _Feeds.GetFixtures("en").Result;
                    if (httpResponse.Meta.RetVal == 1)
                    {
                        if (httpResponse.Data != null)
                            fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                    }
                    List<int> _MatchIds = new List<int>();
                    _MatchIds = fixtures.Where(c => c.GamedayId == matchdays.GamedayId).Select(x => x.MatchId).ToList();
                    foreach (int mMatchId in _MatchIds)
                    {
                        int retval = await _Ingestion.Questions(mMatchId);
                        Catcher("Match Questions ingestion completed for matchId : " + mMatchId + " retVal " + retval);
                    }

                    Catcher("Match Questions ingestion completed. leaderboard ingestion started.");

                    await _Ingestion.LeaderBoard(matchdays.GamedayId, matchdays.PhaseId);

                    Catcher("General leaderboard ingestion completed. Teams recent recent results ingestion started.");

                    await _Ingestion.GetRecentResults();

                    Catcher("Teams recent recent results ingestion completed. Fixtures ingestion started again.");


                    await _Ingestion.Fixtures();

                    Catcher("Fixtures ingestion completed again. CurrentGamedayMatches ingestion started.");


                    await _Ingestion.CurrentGamedayMatches();

                    Catcher("CurrentGamedayMatches ingestion completed.");

                    Catcher("Service Sleeping");
                    Thread.Sleep(_NotificationsDelaySeconds);
                    Catcher("Sending Notification For Point Calculations");
                    await SendPushNotification(4, null);

                }

                Catcher("Points calculation process fully completed.");

                CalculationNotify(matchdays.GamedayId, retVal, rp);


            }
            catch (Exception ex)
            { Catcher("Run", LogLevel.Error, ex); }
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

        private async Task<bool> SendPushNotification(int EventType, int? MatchId)
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
                        Message = message,
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
                        Message = message,
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
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Point Calculation
                case 4:
                    message = notificationText.postMatch;
                    n = new List<NotificationMessages>();
                    n.Add(new NotificationMessages
                    {
                        EventId = (int)NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, leaderboard: _LeaderBoardType);
                    break;
            }
            return success;
        }

        #region " Helper Functions  "

        #region " Notification body "

        private void CalculationNotify(int matchdayId, long result, StringBuilder reports)
        {
            try
            {
                string caption = $"{_Service} [MatchdayId: {matchdayId}]";
                string remark = result == 1 ? "SUCCESS" : "FAILED";

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += $"DCF Fantasy - [ {_Service} RetVal: {result} ]<br/><br/><br/>";
                content += reports.ToString();

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion



        public async Task<int> IngestThis(int matchId)
        {
            int retval = await _Ingestion.Fixtures();
            retval = await _Ingestion.MatchInningStatus(matchId);
            retval = await _Ingestion.CurrentGamedayMatches();
            return retval;
        }

        #endregion

        #region " Notification body "

        private void LockNotify(LockList list, int lockResult, int loadResult)
        {
            try
            {
                string caption = "Game Locking [MatchdayIds: " + string.Join(",", list.MatchdayIdList) + " MatchIds: " + string.Join(",", list.MatchIdList) + "]";
                string remark = lockResult == 1 ? "SUCCESS" : "FAILED";

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "Livepools Fantasy - [ Lock RetVal: " + lockResult + " -- Load RetVal: " + loadResult + " ]<br/>";

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
                content += "Livepools Fantasy - [ Live RetVal: " + lockResult + " ]<br/>";

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion

    }
}
