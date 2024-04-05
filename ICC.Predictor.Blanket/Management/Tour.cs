using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Data;

namespace ICC.Predictor.Blanket.Management
{
    public class Tour : Common.BaseBlanket
    {
        private readonly DataAccess.Management.Tour _DBContext;
        private readonly int _TourId;

        public Tour(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Management.Tour(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public DataTable GetTournaments()
        {
            int optType = 1;
            return _DBContext.GetTournaments(optType, _TourId);
        }
    }
}