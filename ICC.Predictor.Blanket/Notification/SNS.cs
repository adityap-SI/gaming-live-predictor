using Amazon;
using Amazon.SimpleNotificationService.Model;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.AWS;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Notification
{
    class SNS : Common.BaseBlanket
    {
        private readonly DataAccess.Feeds.Gameplay _DBContext;
        private readonly int _TourId;
        private readonly string _IOSApplicationARN;
        private readonly string _AndroidApplicationARN;
        protected RegionEndpoint _AWSSNSRegion;
        private bool _UseCredentials;

        public SNS(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Feeds.Gameplay(postgre);
            _TourId = appSettings.Value.Properties.TourId;
            _IOSApplicationARN = appSettings.Value.Connection.AWS.IOSARN;
            _AndroidApplicationARN = appSettings.Value.Connection.AWS.AndroidARN;
            _AWSSNSRegion = RegionEndpoint.USEast1;
            _UseCredentials = appSettings.Value.Connection.AWS.UseCredentials;
        }

        #region " Subscription - Application "

        public async Task<string> SubscribeToApplication(NotificationPlatforms platform, string deviceToken)
        {
            bool success = false;
            string platformEndpoint = "";
            try
            {
                string ARN = string.Empty;
                string response = string.Empty;

                if (platform == NotificationPlatforms.Android)
                    ARN = _AndroidApplicationARN;
                else if (platform == NotificationPlatforms.IOS)
                    ARN = _IOSApplicationARN;

                if (ARN == "")
                    throw new Exception("Application ARN is empty.");

                CreatePlatformEndpointRequest endpointRequest = new CreatePlatformEndpointRequest();
                endpointRequest.PlatformApplicationArn = ARN;
                endpointRequest.Token = deviceToken;

                Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient _SNSClient = SNSClient();

                CreatePlatformEndpointResponse endpointResponse = await _SNSClient.CreatePlatformEndpointAsync(endpointRequest);

                response = endpointResponse.HttpStatusCode.ToString();

                if (response.ToLower() == "ok")
                {
                    platformEndpoint = endpointResponse.EndpointArn;
                    success = true;
                }
                else
                {
                    platformEndpoint = "";
                    success = false;
                }
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.SubscribeToApplication:", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return platformEndpoint;
        }

        public async Task<bool> UnsubscribeToApplication(NotificationPlatforms platform, string platformEndpoint)
        {
            bool success = false;

            try
            {
                string ARN = string.Empty;
                string response = string.Empty;

                if (platform == NotificationPlatforms.Android)
                    ARN = _AndroidApplicationARN;
                else if (platform == NotificationPlatforms.IOS)
                    ARN = _IOSApplicationARN;

                if (ARN == "")
                    throw new Exception("Application ARN is empty.");

                DeleteEndpointRequest endpointRequest = new DeleteEndpointRequest();
                endpointRequest.EndpointArn = platformEndpoint;

                Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient _SNSClient = SNSClient();

                DeleteEndpointResponse endpointResponse = await _SNSClient.DeleteEndpointAsync(endpointRequest);

                response = endpointResponse.HttpStatusCode.ToString();

                if (response.ToLower() == "ok")
                    success = true;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.UnsubscribeToApplication: ", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return success;
        }

        #endregion

        #region " Subscription - Topic "

        public async Task<string> SubscribeToTopic(string platformEndpoint, string topicArn)
        {
            bool success = false;
            string subscriptionARN = "";
            try
            {
                if (string.IsNullOrEmpty(topicArn))
                    return subscriptionARN;

                string response = string.Empty;

                SubscribeRequest subscribeRequest = new SubscribeRequest();
                subscribeRequest.Endpoint = platformEndpoint;
                subscribeRequest.Protocol = "application";
                subscribeRequest.TopicArn = topicArn;

                Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient _SNSClient = SNSClient();

                SubscribeResponse subscribeResponse = await _SNSClient.SubscribeAsync(subscribeRequest);
                response = subscribeResponse.HttpStatusCode.ToString();

                if (response.ToLower() == "ok")
                {
                    subscriptionARN = subscribeResponse.SubscriptionArn;
                    success = true;
                }
                else
                {
                    subscriptionARN = "";
                    success = false;
                }
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.SubscribeToTopic: ", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return subscriptionARN;
        }

        public async Task<bool> UnsubscribeToTopic(string subscriptionARN)
        {
            bool success = false;

            try
            {
                string response = string.Empty;

                UnsubscribeRequest unsubscribeRequest = new UnsubscribeRequest();
                unsubscribeRequest.SubscriptionArn = subscriptionARN;

                Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient _SNSClient = SNSClient();

                UnsubscribeResponse unsubscribeResponse = await _SNSClient.UnsubscribeAsync(unsubscribeRequest);
                response = unsubscribeResponse.HttpStatusCode.ToString();

                if (response.ToLower() == "ok")
                    success = true;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.UnsubscribeToTopic: ", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return success;
        }

        #endregion

        #region " Publishing - Topic "

        public async Task<bool> PublishToTopic(string message, string subject, string topicArn, NotificationPlatforms platform, long eventId, int? MatchId, string leaderboard)
        {
            bool success = false;

            try
            {
                if (string.IsNullOrEmpty(topicArn))
                    return success;

                // Library.Write.Log.File("topicArn: " + topicArn + " ---- platform: " + platform.ToString() + " ---- eventId: " + eventId);

                string data = string.Empty;

                Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient _SNSClient = SNSClient();

                PublishRequest publishRequest = new PublishRequest();

                if (platform == NotificationPlatforms.Android)
                    data = AndroidMessage(message, subject, eventId, MatchId, leaderboard);
                else
                    data = iOSMessage(message, subject, eventId, MatchId, leaderboard);

                publishRequest.Message = data;
                publishRequest.Subject = subject;
                publishRequest.MessageStructure = "json";
                publishRequest.TopicArn = topicArn;

                PublishResponse resultResponse = await _SNSClient.PublishAsync(publishRequest);

                if (resultResponse.HttpStatusCode.ToString().ToLower() == "ok")
                    success = true;
                else
                    success = false;

                //if (topicArn.ToLower().Trim().EndsWith("_Prod") && IsBackgroundService)
                //    System.Threading.Thread.Sleep(300000);//Waiting 5 minutes
            }
            catch (Exception ex)
            {
                success = false;
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.PublishToTopic: ", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return success;
        }

        private string AndroidMessage(string message, string subject, long eventId, int? matchid, string leaderboard)
        {
            string data = string.Empty;

            try
            {
                var structure = new
                {
                    message,
                    subject,
                    EventId = eventId,
                    matchId = matchid,
                    leaderboard
                };

                var objectData = new
                {
                    data = structure
                };

                var structMessage = new
                {
                    @default = message,
                    GCM = GenericFunctions.Serialize(objectData).ToString()
                };

                data = GenericFunctions.Serialize(structMessage);
            }
            catch (Exception ex)
            {
                data = "";
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.AndroidMessage: ", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return data;
        }

        private string iOSMessage(string message, string subject, long eventId, int? matchid, string leaderboard)
        {
            string data = string.Empty;

            try
            {
                var structure = new
                {
                    alert = message,
                    subject,
                    @event = eventId,
                    @matchId = matchid,
                    leaderboard,
                    sound = "cat.caf"
                };

                var objectData = new
                {
                    aps = structure
                };

                var structMessage = new
                {
                    @default = message,
                    APNS_SANDBOX = GenericFunctions.Serialize(objectData).ToString(),
                    APNS = GenericFunctions.Serialize(objectData).ToString()
                };

                data = GenericFunctions.Serialize(structMessage);
            }
            catch (Exception ex)
            {
                data = "";
                HTTPLog httpLog = _Cookies.PopulateLog("Engine.Notification.SNS.iOSMessage: ", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            return data;
        }

        #endregion

        public Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient SNSClient()
        {
            if (_UseCredentials)
                return new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(Credentials._AWSCredentials, _AWSSNSRegion);
            else
                return new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(_AWSSNSRegion);
        }

    }
}
