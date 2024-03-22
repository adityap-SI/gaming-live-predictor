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
    public class Subscription : Common.BaseBlanket
    {
        private readonly DataAccess.Notification.Subscription _DBSubscriptionContext;
        private readonly Blanket.Notification.SNS _BlanketSNSContext;
        private readonly Blanket.Notification.Topics _BlanketTopicsContext;
        private readonly Int32 _TourId;

        public Subscription(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBSubscriptionContext = new DataAccess.Notification.Subscription(postgre);
            _BlanketSNSContext = new Blanket.Notification.SNS(appSettings, aws, postgre, redis, cookies, asset);
            _BlanketTopicsContext = new Blanket.Notification.Topics(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public async Task<HTTPResponse> Subscriptions(Contracts.Notification.Subscription subscription, String language)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            List<Contracts.Notification.Topics> topics = new List<Contracts.Notification.Topics>();
            Int64 retVal = -40;

            try
            {
                if (_Cookies._HasUserCookies)
                {
                    Int32 _userId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    Int32 UserTourTeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    HTTPResponse mHTTPResponse = await _BlanketTopicsContext.TopicsGet(1);
                    topics = GenericFunctions.Deserialize<List<Contracts.Notification.Topics>>(GenericFunctions.Serialize(((ResponseObject)mHTTPResponse.Data).Value));
                    subscription.EventId = topics.Where(c => c.PlatformId == (Int32)subscription.Platform).Select(o => o.EventId).FirstOrDefault();

                    if (subscription.IsActive == 1)
                    {
                        String platformEndpoint = "", subscriptionARN = "";

                        Tuple<bool, string, string> mSubscriptionReponse = await AWSSubscribe(subscription, language);

                        
                            //subscription.PlatformEndpoint = platformEndpoint;
                            //subscription.SubscriptionArn = subscriptionARN;
                            subscription.PlatformEndpoint = mSubscriptionReponse.Item2;
                            subscription.SubscriptionArn = mSubscriptionReponse.Item3;


                        if (mSubscriptionReponse.Item1 == true)
                        {
                            Int32 optType = 1;
                            NotificationDetails notification = _DBSubscriptionContext.Subscriptions(optType, _TourId, _userId, UserTourTeamId, subscription.DeviceToken, (Int32)subscription.Platform,
                                subscription.DeviceIdentity, (subscription.EnableNotification ? 1 : 0),
                                language, subscription.PlatformEndpoint, subscription.SubscriptionArn, subscription.IsActive, subscription.EventId, ref httpMeta);
                            retVal = httpMeta.RetVal;
                        }
                    }
                    else
                    {
                        Int32 optType = 2;
                        NotificationDetails notification = _DBSubscriptionContext.Subscriptions(optType, _TourId, _userId, UserTourTeamId, subscription.DeviceToken, (Int32)subscription.Platform,
                           subscription.DeviceIdentity, (subscription.EnableNotification ? 1 : 0),
                           language, "", "", subscription.IsActive, subscription.EventId, ref httpMeta);

                        retVal = httpMeta.RetVal;

                        if (retVal == 1)
                        {
                            if (notification != null && !String.IsNullOrEmpty(notification.PlatformEndpoint))
                            {
                                bool success = await  AWSUnsubscribe(subscription.Platform, notification.PlatformEndpoint, notification.SubscriptionARN);
                                retVal = (success) ? 1 : -10;
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
            String language)
        {
            Int64 retVal = -40;
            Int32 _optType = 1;
            HTTPMeta httpMeta = new HTTPMeta();
            HTTPResponse hTTPResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();

            try
            {
                if (_Cookies._HasUserCookies)
                {
                    Int32 _userId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    Int32 UserTourTeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    DeviceUpdate dv = _DBSubscriptionContext.DeviceUpdate(_optType, _TourId, _userId, UserTourTeamId,  subscription.DeviceToken,
                    subscription.PlatformEndpoint,(Int32)subscription.Platform, subscription.DeviceIdentity, ref httpMeta);

                    if (httpMeta.RetVal == 1)
                    {
                        retVal = httpMeta.RetVal;

                        if (dv != null && dv.toUnsubscribe != null && dv.toUnsubscribe.Any() && !String.IsNullOrEmpty(dv.toUnsubscribe[0].PlatformEndpoint))
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
                                success = (hTTPResponse.Meta.RetVal == 1);

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

        public HTTPResponse EventsGet(String deviceId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject res = new ResponseObject();

            try
            {
                if (_Cookies._HasUserCookies)
                {
                    Int32 _optType = 1;
                    Int32 UserTourTeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
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

        private async Task<Tuple<Boolean, String, String>> AWSSubscribe(Contracts.Notification.Subscription subscription, String language)
        {
            bool success = false;
            String platformEndpoint = "";
            String subscriptionARN = "";

            try
            {
                //Subscribe user to Application
                platformEndpoint = await _BlanketSNSContext.SubscribeToApplication(subscription.Platform, subscription.DeviceToken);

                if (String.IsNullOrEmpty(platformEndpoint))
                    throw new Exception("Error while subscribing user to Application");

                Contracts.Notification.Topics tp = await _BlanketTopicsContext.TopicByFilter(subscription.EventId, subscription.Platform, language);

                //Subscribe user to Topic
                subscriptionARN = await _BlanketSNSContext.SubscribeToTopic(platformEndpoint, tp.EventTopicARN);

                if (String.IsNullOrEmpty(subscriptionARN))
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

        private async Task<bool> AWSUnsubscribe(NotificationPlatforms platform, String platformEndpoint, String subscriptionARN)
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
