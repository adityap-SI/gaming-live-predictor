using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Contracts.Notification;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodog.Predictor.Blanket.Notification
{
    public class Update : Common.BaseBlanket
    {
        private readonly DataAccess.Notification.Update _DBUpdateContext;
        private readonly Int32 _TourId;

        public Update(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBUpdateContext = new DataAccess.Notification.Update(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }
        public Int64 Insert(Int64 optType, Int64 matchday, Int64 gamedayId, out String error)
        {
            error = "";
            Int64 retVal = -40;
            
            try
            {
                retVal = _DBUpdateContext.Insert(optType, _TourId, matchday, gamedayId);
            }
            catch (Exception ex)
            {
                error = "Blanket.Notification.Update.Insert: " + ex.Message;
            }

            return retVal;
        }

        public Int64 UpdateStatus(Int64 notificationId, out String error)
        {
            error = "";
            Int64 retVal = -40;
           

            try
            {
                Int64 optType = 1;

                retVal = _DBUpdateContext.UpdateStatus(optType, _TourId, notificationId);
            }
            catch (Exception ex)
            {
                error = "Blanket.Notification.Update.UpdateStatus: " + ex.Message;
            }

            return retVal;
        }
    }
}
