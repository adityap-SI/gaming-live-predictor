using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ICC.Predictor.Blanket.Management
{
    public class Series : Common.BaseBlanket
    {
        private readonly DataAccess.Management.Series _DBContext;
        private readonly int _TourId;

        public Series(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
           : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Management.Series(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public DataTable GetSeries(int tournamentId)
        {
            int optType = 1;
            return _DBContext.GetSeries(optType, _TourId, tournamentId);
        }
    }
}
