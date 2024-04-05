using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Notification
{
    public class Topics : Common.BaseBlanket
    {
        private readonly DataAccess.Notification.Subscription _DBSubscriptionContext;
        private readonly int _TourId;

        public Topics(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBSubscriptionContext = new DataAccess.Notification.Subscription(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public async Task<HTTPResponse> UniqueEvents(int optType, bool offloadDb = true)
        {

            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject res = new ResponseObject();

            try
            {

                if (offloadDb)
                {

                    string data = await _Asset.GET(_Asset.UniqueEvents());

                    res = GenericFunctions.Deserialize<ResponseObject>(data);

                    int retVal = res != null ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta);
                }
                else
                    res = _DBSubscriptionContext.UniqueEvents(optType, _TourId, ref httpMeta);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Notification.Topics.UniqueEvents: " + ex.Message);
            }

            httpResponse.Data = res;
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> TopicsGet(int optType, bool offloadDb = true)
        {

            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject res = new ResponseObject();

            try
            {

                if (offloadDb)
                {

                    string data = await _Asset.GET(_Asset.NotificationTopics());

                    res = GenericFunctions.Deserialize<ResponseObject>(data);

                    int retVal = res != null ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
                else
                    res = _DBSubscriptionContext.TopicsGet(optType, _TourId, ref httpMeta);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Notification.Topics.TopicsGet: " + ex.Message);
            }

            httpResponse.Data = res;
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<Contracts.Notification.Topics> TopicByFilter(long eventId, NotificationPlatforms platform, string language)
        {
            Contracts.Notification.Topics tp = new Contracts.Notification.Topics();

            try
            {
                int _optType = 1;
                HTTPResponse response = await TopicsGet(_optType);

                if (response.Meta.RetVal == 1)
                {
                    List<Contracts.Notification.Topics> topics = new List<Contracts.Notification.Topics>();

                    if (response.Data != null)
                        topics = GenericFunctions.Deserialize<List<Contracts.Notification.Topics>>(GenericFunctions.Serialize(((ResponseObject)response.Data).Value));
                    else
                        throw new Exception("httpResponse.Data is NULL.");

                    if (topics != null && topics.Any())
                        tp = topics.Where(o => o.EventId == eventId && o.Language.ToLower() == language.ToLower() && o.PlatformId == (int)platform).FirstOrDefault();

                    if (tp == null)
                        throw new Exception("Topic ARN not found for the given Event, Language and Platform");
                }
                else
                    throw new Exception("Topic RetVal not equal to 1. RetVal = " + response.Meta.RetVal);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Notification.Topics.TopicsGet: " + ex.Message);
            }

            return tp;
        }

        public async Task<NotificationText> GetNotificationText()
        {
            NotificationText mNotificationText = new NotificationText();

            string data = await _Asset.GET(_Asset.NotificationText());

            mNotificationText = GenericFunctions.Deserialize<NotificationText>(data);

            return mNotificationText;
        }

        public async Task<bool> GetMatchNotificationStatus(int matchId)
        {
            bool status = false;
            try
            {

                List<NotificationStatus> mNotificationStatus = new List<NotificationStatus>();

                string data = await _Asset.GET(_Asset.NotificationStatus());

                mNotificationStatus = GenericFunctions.Deserialize<List<NotificationStatus>>(data);

                NotificationStatus mStatus = mNotificationStatus.Where(x => x.MatchId == matchId).FirstOrDefault();
                if (mStatus.PreMatchNotification == true)
                    status = true;

            }
            catch (Exception ex) { }

            return status;
        }

        public async Task<bool> UpdateMatchNotificationStatus(int matchId)
        {
            bool status = false;
            try
            {

                List<NotificationStatus> mNotificationStatus = new List<NotificationStatus>();

                string data = await _Asset.GET(_Asset.NotificationStatus());

                mNotificationStatus = GenericFunctions.Deserialize<List<NotificationStatus>>(data);

                List<NotificationStatus> mNotificationStatusesUpdated = new List<NotificationStatus>();
                foreach (NotificationStatus mMatchNotification in mNotificationStatus)
                {
                    if (mMatchNotification.MatchId == matchId)
                        mMatchNotification.PreMatchNotification = true;

                    mNotificationStatusesUpdated.Add(mMatchNotification);
                }

                status = await _Asset.SET(_Asset.NotificationStatus(), mNotificationStatusesUpdated);

            }
            catch (Exception ex) { }

            return status;
        }

    }
}
