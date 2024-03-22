using Bodog.Predictor.Interfaces.AWS;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bodog.Predictor.Contracts.BackgroundServices;
using System;
using System.Threading;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.Session;
using System.Collections.Generic;
using Bodog.Predictor.Contracts.Feeds;
using System.Linq;
using Bodog.Predictor.Library.Utility;
using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Contracts.Automate;
using System.Data;
using System.Text;

namespace Bodog.Predictor.Daemon.BackgroundServices
{
    public class PointsCalculation : BaseService<PointsCalculation>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.BackgroundServices.PointsCalculation _Calculation;
        private readonly Blanket.Feeds.Gameplay _Feeds;
        private Blanket.Feeds.Ingestion _Ingestion;
        private Blanket.Notification.Topics _NotificationTopics;
        private Blanket.Notification.Publish _NotificationPublish;
        private Int32 _Interval;
        private Int32 _NotificationsDelaySeconds;
        private String _LeaderBoardType;

        public PointsCalculation(ILogger<PointsCalculation> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _Calculation = new Blanket.BackgroundServices.PointsCalculation(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Feeds = new Blanket.Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationTopics = new Blanket.Notification.Topics(appSettings, aws, postgre, redis, cookies, asset);
            _NotificationPublish = new Blanket.Notification.Publish(appSettings, aws, postgre, redis, cookies, asset);
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
                Boolean success = false;

                Matchdays matchdays = new Matchdays();
                matchdays = _Calculation.Matchdays();
                //if (matchdays == null || matchdays.GamedayId == 0 || matchdays.Matchday == 0)
                if (matchdays == null || matchdays.GamedayId == 0)
                    return;

                Catcher("MatchdayId found: " + GenericFunctions.Serialize(matchdays));


                Catcher("Points calculation process started for gameDayId " + matchdays.GamedayId + " matchdayId " + matchdays.Matchday);

                Int32 retVal = _Calculation.UserPointsProcess(matchdays.GamedayId, matchdays.Matchday);

                Catcher("Points calculation process completed. Retval: " + retVal);

                Catcher("Reports process started.");
                DataSet ds = _Calculation.UserPointsProcessReports(retVal, matchdays.GamedayId, matchdays.Matchday);

                StringBuilder rp = _Calculation.ParseReports(ds);

                Catcher("Reports process completed.");

                if (retVal == 1)
                {

                    Catcher("Match Questions ingestion started");

                    List<Fixtures> fixtures = new List<Fixtures>();
                    Contracts.Common.HTTPResponse httpResponse = _Feeds.GetFixtures("en").Result;
                    if (httpResponse.Meta.RetVal == 1)
                    {
                        if (httpResponse.Data != null)
                            fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((Contracts.Common.ResponseObject)httpResponse.Data).Value));
                    }
                    List<Int32> _MatchIds = new List<Int32>();
                    _MatchIds = fixtures.Where(c => c.GamedayId == matchdays.GamedayId).Select(x => x.MatchId).ToList();
                    foreach (Int32 mMatchId in _MatchIds)
                    {
                        Int32 retval = await _Ingestion.Questions(mMatchId);
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
                    System.Threading.Thread.Sleep(_NotificationsDelaySeconds);
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

        private async Task<bool> SendPushNotification(Int32 EventType, Int32? MatchId)
        {
            Contracts.Notification.NotificationText notificationText = new Contracts.Notification.NotificationText();
            notificationText = await _NotificationTopics.GetNotificationText();
            String message = "";
            bool success = false;
            switch (EventType)
            {
                // Notification Before Match Lock
                case 1:
                    message = notificationText.preMatch;
                    List<Contracts.Notification.NotificationMessages> n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Notification 1st Inning Unlock
                case 2:
                    message = notificationText.openInningOne;
                    n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Notification 2nd Inning Unlock
                case 3:
                    message = notificationText.openInningTwo;
                    n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
                        Language = "en",
                        Message = message,
                        Subject = "Generic"
                    });
                    success = await _NotificationPublish.Messages(true, true, n, MatchId);
                    break;
                // Point Calculation
                case 4:
                    message = notificationText.postMatch;
                    n = new List<Contracts.Notification.NotificationMessages>();
                    n.Add(new Contracts.Notification.NotificationMessages
                    {
                        EventId = (Int32)Contracts.Notification.NotificationEvents.Generic,
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

        private void CalculationNotify(Int32 matchdayId, Int64 result, StringBuilder reports)
        {
            try
            {
                string caption = $"{_Service} [MatchdayId: {matchdayId}]";
                string remark = ((result == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += $"DCF Fantasy - [ {_Service} RetVal: {result} ]<br/><br/><br/>";
                content += reports.ToString();

                String body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion



        public async Task<int> IngestThis(Int32 matchId)
        {
            Int32 retval = await _Ingestion.Fixtures();
            retval = await _Ingestion.MatchInningStatus(matchId);
            retval = await _Ingestion.CurrentGamedayMatches();
            return retval;
        }

        #endregion

        #region " Notification body "

        private void LockNotify(LockList list, Int32 lockResult, Int32 loadResult)
        {
            try
            {
                string caption = "Game Locking [MatchdayIds: " + string.Join(",", list.MatchdayIdList) + " MatchIds: " + string.Join(",", list.MatchIdList) + "]";
                string remark = ((lockResult == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "Livepools Fantasy - [ Lock RetVal: " + lockResult + " -- Load RetVal: " + loadResult + " ]<br/>";

                String body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        private void LiveNotify(LockList list, Int32 lockResult)
        {
            try
            {
                string caption = "Game Live [MatchdayIds: " + string.Join(",", list.MatchdayIdList) + " MatchIds: " + string.Join(",", list.MatchIdList) + "]";
                string remark = ((lockResult == 1) ? "SUCCESS" : "FAILED");

                string content = "Remark: " + caption + " - " + remark + "<br/>";
                content += "Livepools Fantasy - [ Live RetVal: " + lockResult + " ]<br/>";

                String body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        #endregion

    }
}
