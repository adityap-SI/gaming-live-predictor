using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bodog.Predictor.Blanket.Management
{
    public class Series : Common.BaseBlanket
    {
        private readonly DataAccess.Management.Series _DBContext;
        private readonly Int32 _TourId;

        public Series(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
           : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Management.Series(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public DataTable GetSeries(Int32 tournamentId)
        {
            Int32 optType = 1;
            return _DBContext.GetSeries(optType, _TourId, tournamentId);
        }
    }
}
