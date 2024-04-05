using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ICC.Predictor.Contracts.BackgroundServices;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ICC.Predictor.Contracts.Feeds;
using System.Linq;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Automate;
using System.Data;
using System.Text;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Daemon.BackgroundServices
{
    class PeriodicUpdate : BaseService<PeriodicUpdate>, IHostedService, IDisposable
    {
        private Timer _Timer;
        private Blanket.BackgroundServices.PeriodicUpdate _PeriodicUpdate;
        private Blanket.Feeds.Ingestion _Ingestion;
        private int _Interval;

        public PeriodicUpdate(ILogger<PeriodicUpdate> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings,
           IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset) : base(logger, appSettings, serviceSettings, aws, postgre, redis, asset)
        {
            _PeriodicUpdate = new Blanket.BackgroundServices.PeriodicUpdate(appSettings, serviceSettings, aws, postgre, redis, cookies, asset);
            _Ingestion = new Blanket.Feeds.Ingestion(appSettings, aws, postgre, redis, cookies, asset);
            _Interval = serviceSettings.Value.PeriodicUpdate.IntervalMinutes;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Catcher("Started.");

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
                int selectionRetVal = 0;
                int partitionRetVal = _PeriodicUpdate.PartitionUpdate(1, _TourId, 0);

                Catcher($"Iteration completed. Partition RetVal: {partitionRetVal}");
            }
            catch (Exception ex)
            {
                Catcher("Run", LogLevel.Error, ex);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Timer?.Change(Timeout.Infinite, 0);

            Catcher("Stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }

    }
}
