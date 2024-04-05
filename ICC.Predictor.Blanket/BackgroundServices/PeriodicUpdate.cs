using System;
using System.Linq;
using ICC.Predictor.Contracts.Common;
using Microsoft.Extensions.Options;
using ICC.Predictor.Contracts.Feeds;
using System.Collections.Generic;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Contracts.BackgroundServices;
using ICC.Predictor.Contracts.Admin;
using System.Net;
using System.Xml.Linq;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Blanket.BackgroundServices
{
    public class PeriodicUpdate : Common.BaseServiceBlanket
    {

        private readonly Feeds.Gameplay _Feeds;
        private readonly DataAccess.BackgroundServices.PeriodicUpdate _PeriodicUpdateContext;
        private readonly int _TourId;

        public PeriodicUpdate(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
         : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            _Feeds = new Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _PeriodicUpdateContext = new DataAccess.BackgroundServices.PeriodicUpdate(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public int PartitionUpdate(int optType, int matchId, int gamedayId)
        {
            return _PeriodicUpdateContext.PartitionUpdate(optType, _TourId, gamedayId);
        }
    }
}
