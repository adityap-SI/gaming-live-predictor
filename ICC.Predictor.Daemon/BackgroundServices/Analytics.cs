using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Daemon.BackgroundServices
{
    class Analytics : BaseService<Analytics>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.Analytics.Analytics _Analytics;
        private int _Interval;

        public Analytics(ILogger<Analytics> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _Analytics = new Blanket.Analytics.Analytics(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.Analytics.IntervalMinutes;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Catcher("Analytics Started.");

            //Timer runs immediately. Periodic intervals is disabled.
            _Timer = new Timer(Process, null, 0, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private void Process(object state)
        {
            Run(state);

            //Timer runs after the interval period. Periodic intervals is disabled.
            _Timer?.Change(Convert.ToInt32(TimeSpan.FromHours(_Interval).TotalMilliseconds), Timeout.Infinite);
        }

        private async void Run(object state)
        {
            try
            {
                Catcher("Analytics initiated.");
                int RetVal = 0;
                string error = string.Empty;
                string analytics = _Analytics.GetAnalytics(ref error);
                if (!string.IsNullOrEmpty(analytics))
                    AnalyticsNotify(RetVal, analytics);
                else
                    Catcher("Analytics is null." + error);
            }
            catch (Exception ex)
            {
                Catcher("Analytics Run", LogLevel.Error, ex);
            }
        }

        private void AnalyticsNotify(long result, string reports)
        {
            try
            {
                string caption = $"{_Service} [Analytics]";

                string content = "ICC Analytics<br/>";
                content += reports;

                string body = GenericFunctions.EmailBody(_Service, content);
                Notify(caption, body);
            }
            catch { }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);

            Catcher("Analytics Stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
