using Bodog.Predictor.Interfaces.AWS;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.Session;
using System.Collections.Generic;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Library.Utility;

namespace Bodog.Predictor.Daemon.BackgroundServices
{
    class Analytics : BaseService<Analytics>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.Analytics.Analytics _Analytics;
        private Int32 _Interval;

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
                Int32 RetVal = 0;
                String error = String.Empty;
                String analytics = _Analytics.GetAnalytics(ref error);
                if (!String.IsNullOrEmpty(analytics))
                    AnalyticsNotify(RetVal, analytics);
                else
                    Catcher("Analytics is null." + error);
            }
            catch (Exception ex)
            {
                Catcher("Analytics Run", LogLevel.Error, ex);
            }
        }

        private void AnalyticsNotify(Int64 result, String reports)
        {
            try
            {
                string caption = $"{_Service} [Analytics]";

                string content = "Bodog Analytics<br/>";
                content += reports;

                String body = GenericFunctions.EmailBody(_Service, content);
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
