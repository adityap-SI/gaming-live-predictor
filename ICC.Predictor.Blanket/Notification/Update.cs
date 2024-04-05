using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Notification
{
    public class Update : BaseBlanket
    {
        private readonly DataAccess.Notification.Update _DBUpdateContext;
        private readonly int _TourId;

        public Update(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBUpdateContext = new DataAccess.Notification.Update(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }
        public long Insert(long optType, long matchday, long gamedayId, out string error)
        {
            error = "";
            long retVal = -40;

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

        public long UpdateStatus(long notificationId, out string error)
        {
            error = "";
            long retVal = -40;


            try
            {
                long optType = 1;

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
