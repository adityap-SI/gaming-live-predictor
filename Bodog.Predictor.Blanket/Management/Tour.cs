using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Data;

namespace Bodog.Predictor.Blanket.Management
{
    public class Tour : Common.BaseBlanket
    {
        private readonly DataAccess.Management.Tour _DBContext;
        private readonly Int32 _TourId;

        public Tour(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Management.Tour(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public DataTable GetTournaments()
        {
            Int32 optType = 1;
            return _DBContext.GetTournaments(optType, _TourId);
        }
    }
}