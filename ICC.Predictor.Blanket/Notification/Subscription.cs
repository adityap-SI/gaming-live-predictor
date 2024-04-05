using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Blanket.Common;
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
    public class Subscription : BaseBlanket
    {
        private readonly DataAccess.Notification.Subscription _DBSubscriptionContext;
        private readonly SNS _BlanketSNSContext;
        private readonly Topics _BlanketTopicsContext;
        private readonly int _TourId;

        public Subscription(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBSubscriptionContext = new DataAccess.Notification.Subscription(postgre);
            _BlanketSNSContext = new SNS(appSettings, aws, postgre, redis, cookies, asset);
            _BlanketTopicsContext = new Topics(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public async Task<HTTPResponse> Subscriptions(Contracts.Notification.Subscription subscription, string language)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            List<Contracts.Notification.Topics> topics = new List<Contracts.Notification.Topics>();
            long retVal = -40;

            try
            {
                if (_Cookies._HasUserCookies)
                {
                    int _userId = int.Parse(_Cookies._GetUserCookies.UserId);
                    int UserTourTeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    HTTPResponse mHTTPResponse = await _BlanketTopicsContext.TopicsGet(1);
                    topics = GenericFunctions.Deserialize<List<Contracts.Notification.Topics>>(GenericFunctions.Serialize(((ResponseObject)mHTTPResponse.Data).Value));
                    subscription.EventId = topics.Where(c => c.PlatformId == (int)subscription.Platform).Select(o => o.EventId).FirstOrDefault();

                    if (subscription.IsActive == 1)
                    {
                        string platformEndpoint = "", subscriptionARN = "";

                        Tuple<bool, string, string> mSubscriptionReponse = await AWSSubscribe(subscription, language);


                        //subscription.PlatformEndpoint = platformEndpoint;
                        //subscription.SubscriptionArn = subscriptionARN;
                        subscription.PlatformEndpoint = mSubscriptionReponse.Item2;
                        subscription.SubscriptionArn = mSubscriptionReponse.Item3;


                        if (mSubscriptionReponse.Item1 == true)
                        {
                            int optType = 1;
                            NotificationDetails notification = _DBSubscriptionContext.Subscriptions(optType, _TourId, _userId, UserTourTeamId, subscription.DeviceToken, (int)subscription.Platform,
                                subscription.DeviceIdentity, subscription.EnableNotification ? 1 : 0,
                                language, subscription.PlatformEndpoint, subscription.SubscriptionArn, subscription.IsActive, subscription.EventId, ref httpMeta);
                            retVal = httpMeta.RetVal;
                        }
                    }
                    else
                    {
                        int optType = 2;
                        NotificationDetails notification = _DBSubscriptionContext.Subscriptions(optType, _TourId, _userId, UserTourTeamId, subscription.DeviceToken, (int)subscription.Platform,
                           subscription.DeviceIdentity, subscription.EnableNotification ? 1 : 0,
                           language, "", "", subscription.IsActive, subscription.EventId, ref httpMeta);

                        retVal = httpMeta.RetVal;

                        if (retVal == 1)
                        {
                            if (notification != null && !string.IsNullOrEmpty(notification.PlatformEndpoint))
                            {
                                bool success = await AWSUnsubscribe(subscription.Platform, notification.PlatformEndpoint, notification.SubscriptionARN);
                                retVal = success ? 1 : -10;
                            }
                            else
                                throw new Exception("PlatformEndpoint is NULL for unsubscribing user from AWS.");
                        }
                    }

                }
                else
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Notifications.Subscription.Subscriptions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            responseObject.Value = retVal;
            responseObject.FeedTime = GenericFunctions.GetFeedTime();
            httpResponse.Data = responseObject;
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> DeviceUpdate(Contracts.Notification.Subscription subscription,
            string language)
        {
            long retVal = -40;
            int _optType = 1;
            HTTPMeta httpMeta = new HTTPMeta();
            HTTPResponse hTTPResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();

            try
            {
                if (_Cookies._HasUserCookies)
                {
                    int _userId = int.Parse(_Cookies._GetUserCookies.UserId);
                    int UserTourTeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    DeviceUpdate dv = _DBSubscriptionContext.DeviceUpdate(_optType, _TourId, _userId, UserTourTeamId, subscription.DeviceToken,
                    subscription.PlatformEndpoint, (int)subscription.Platform, subscription.DeviceIdentity, ref httpMeta);

                    if (httpMeta.RetVal == 1)
                    {
                        retVal = httpMeta.RetVal;

                        if (dv != null && dv.toUnsubscribe != null && dv.toUnsubscribe.Any() && !string.IsNullOrEmpty(dv.toUnsubscribe[0].PlatformEndpoint))
                        {
                            bool success = false;

                            //Unsubscribe all the endpoints
                            foreach (NotificationDetails notification in dv.toUnsubscribe)
                            {
                                success = await AWSUnsubscribe(subscription.Platform, notification.PlatformEndpoint, notification.SubscriptionARN);

                                if (!success)
                                    break;
                            }

                            retVal = success ? 1 : -10;
                        }

                        if (dv != null && dv.toSubscribe != null && dv.toSubscribe.Any() && dv.toSubscribe[0].EventId != 0)
                        {
                            bool success = false;

                            //Subscribe all the deviceTokens
                            foreach (EventDetails events in dv.toSubscribe)
                            {
                                subscription.EventId = events.EventId;
                                subscription.IsActive = events.IsActive;

                                hTTPResponse = await Subscriptions(subscription, language);
                                success = hTTPResponse.Meta.RetVal == 1;

                                if (!success)
                                    break;
                            }

                            retVal = success ? 1 : -20;
                            responseObject.Value = retVal;
                            responseObject.FeedTime = GenericFunctions.GetFeedTime();
                            hTTPResponse.Data = responseObject;
                            hTTPResponse.Meta = httpMeta;
                        }

                        responseObject.Value = retVal;
                        responseObject.FeedTime = GenericFunctions.GetFeedTime();
                        hTTPResponse.Data = responseObject;
                        hTTPResponse.Meta = httpMeta;
                    }
                    else
                        throw new Exception("RetVal not equal to 1. RetVal = " + httpMeta.RetVal);
                }
                else
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Notification.Subscription.DeviceUpdate:", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return hTTPResponse;
        }

        public HTTPResponse EventsGet(string deviceId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject res = new ResponseObject();

            try
            {
                if (_Cookies._HasUserCookies)
                {
                    int _optType = 1;
                    int UserTourTeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    List<Events> events = _DBSubscriptionContext.EventsGet(_optType, UserTourTeamId, deviceId, _TourId, ref httpMeta);

                    res.Value = events;
                    res.FeedTime = GenericFunctions.GetFeedTime();
                }
                else
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Notification.Subscription.EventsGet:", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Data = res;
            httpResponse.Meta = httpMeta;

            return httpResponse;
        }

        private async Task<Tuple<bool, string, string>> AWSSubscribe(Contracts.Notification.Subscription subscription, string language)
        {
            bool success = false;
            string platformEndpoint = "";
            string subscriptionARN = "";

            try
            {
                //Subscribe user to Application
                platformEndpoint = await _BlanketSNSContext.SubscribeToApplication(subscription.Platform, subscription.DeviceToken);

                if (string.IsNullOrEmpty(platformEndpoint))
                    throw new Exception("Error while subscribing user to Application");

                Contracts.Notification.Topics tp = await _BlanketTopicsContext.TopicByFilter(subscription.EventId, subscription.Platform, language);

                //Subscribe user to Topic
                subscriptionARN = await _BlanketSNSContext.SubscribeToTopic(platformEndpoint, tp.EventTopicARN);

                if (string.IsNullOrEmpty(subscriptionARN))
                    throw new Exception("Error while subscribing user to Topic");

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Notification.Subscription.AWSSubscribe:", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return Tuple.Create(success, platformEndpoint, subscriptionARN);
        }

        private async Task<bool> AWSUnsubscribe(NotificationPlatforms platform, string platformEndpoint, string subscriptionARN)
        {
            bool success = false;

            try
            {
                //Unsubscribe user to Application
                success = await _BlanketSNSContext.UnsubscribeToApplication(platform, platformEndpoint);

                if (!success)
                    throw new Exception("Error while unsubscribing user to Application");

                //Unsubscribe user to Topic
                success = await _BlanketSNSContext.UnsubscribeToTopic(subscriptionARN);

                if (!success)
                    throw new Exception("Error while unsubscribing user to Topic");
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Notification.Subscription.AWSUnsubscribe:", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return success;
        }

    }
}
