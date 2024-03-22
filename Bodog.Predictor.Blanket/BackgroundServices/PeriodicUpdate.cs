using System;
using System.Linq;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Common;
using Microsoft.Extensions.Options;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Contracts.Feeds;
using System.Collections.Generic;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Bodog.Predictor.Contracts.BackgroundServices;
using Bodog.Predictor.Contracts.Admin;
using System.Net;
using System.Xml.Linq;

namespace Bodog.Predictor.Blanket.BackgroundServices
{
    public class PeriodicUpdate : Common.BaseServiceBlanket
    {

        private readonly Feeds.Gameplay _Feeds;
        private readonly DataAccess.BackgroundServices.PeriodicUpdate _PeriodicUpdateContext;
        private readonly Int32 _TourId;

        public PeriodicUpdate(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
         : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            _Feeds = new Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _PeriodicUpdateContext = new DataAccess.BackgroundServices.PeriodicUpdate(postgre);
            _TourId = appSettings.Value.Properties.TourId;            
        }

        public Int32 PartitionUpdate(Int32 optType, Int32 matchId, Int32 gamedayId)
        {
            return _PeriodicUpdateContext.PartitionUpdate(optType, _TourId, gamedayId);
        }
    }
}
