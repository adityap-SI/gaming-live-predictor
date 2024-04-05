using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Automate;
using System.Data;
using System.Text;
using ICC.Predictor.Contracts.BackgroundServices;
using ICC.Predictor.Blanket.Scoring;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Blanket.Feeds;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Daemon.BackgroundServices
{
    public class MatchAnswerCalculation : BaseService<MatchAnswerCalculation>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private readonly Process _ScoringContext;
        private readonly Gameplay _Feeds;
        private Ingestion _Ingestion;
        private readonly Blanket.BackgroundServices.MatchAnswerCalculation _MatchAnswerCalculationContext;
        private int _Interval;

        public MatchAnswerCalculation(ILogger<MatchAnswerCalculation> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _ScoringContext = new Process(appSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Feeds = new Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _MatchAnswerCalculationContext = new Blanket.BackgroundServices.MatchAnswerCalculation(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.MatchAnswerCalculation.IntervalMinutes;
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

                List<Fixtures> mFixtures = new List<Fixtures>();
                mFixtures = _MatchAnswerCalculationContext.GetFinishedMatches();
                if (mFixtures != null && mFixtures.Any())
                {

                    foreach (Fixtures fixture in mFixtures)
                    {
                        Catcher("Answers Submission Started For Match Id : " + fixture.MatchId);

                        success = _ScoringContext.CalculateAnswers(fixture);

                        Catcher("Answers Submission Completed For Match Id : " + fixture.MatchId + " Result :" + success);
                        if (success)
                            success = _ScoringContext.QuestionAnswerProcessUpdate(fixture.MatchId);
                        if (success)
                            success = _ScoringContext.SubmitMatchWinTeam(fixture);
                        else
                            break;
                    }


                    Catcher("Fixtures ingestion started.");

                    await _Ingestion.Fixtures();

                    Catcher("Fixtures ingestion completed. CurrentGamedayMatches ingestion started.");

                    await _Ingestion.CurrentGamedayMatches();

                    Catcher("CurrentGamedayMatches ingestion completed. Teams Recent Results igestion started.");

                    await _Ingestion.GetRecentResults();

                    Catcher("Teams Recent Results ingestion completed.");
                }
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
