using Amazon;
using Amazon.SimpleNotificationService.Model;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Contracts.Notification;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.AWS;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodog.Predictor.Blanket.Notification
{
    class SNS : Common.BaseBlanket
    {
        private readonly DataAccess.Feeds.Gameplay _DBContext;
        private readonly Int32 _TourId;
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

        public async Task<String> SubscribeToApplication(NotificationPlatforms platform, String deviceToken)
        {
            bool success = false;
            String platformEndpoint = "";
            try
            {
                String ARN = String.Empty;
                String response = String.Empty;

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

        public async Task<bool> UnsubscribeToApplication(NotificationPlatforms platform, String platformEndpoint)
        {
            bool success = false;

            try
            {
                String ARN = String.Empty;
                String response = String.Empty;

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

        public async Task<String> SubscribeToTopic(String platformEndpoint, String topicArn)
        {
            bool success = false;
            String subscriptionARN = "";
            try
            {
                if (String.IsNullOrEmpty(topicArn))
                    return subscriptionARN;

                String response = String.Empty;

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

        public async Task<bool> UnsubscribeToTopic(String subscriptionARN)
        {
            bool success = false;

            try
            {
                String response = String.Empty;

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

        public async Task<bool> PublishToTopic(String message, String subject, String topicArn, NotificationPlatforms platform, Int64 eventId, Int32? MatchId, String leaderboard)
        {
            bool success = false;

            try
            {
                if (String.IsNullOrEmpty(topicArn))
                    return success;

               // Library.Write.Log.File("topicArn: " + topicArn + " ---- platform: " + platform.ToString() + " ---- eventId: " + eventId);

                String data = String.Empty;

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

        private String AndroidMessage(String message, String subject, Int64 eventId, Int32? matchid, String leaderboard)
        {
            String data = String.Empty;

            try
            {
                var structure = new
                {
                    message = message,
                    subject = subject,
                    EventId = eventId,
                    matchId = matchid,
                    leaderboard = leaderboard
                };

                var objectData = new
                {
                    data = structure
                };

                var structMessage = new
                {
                    @default = message,
                    GCM = (GenericFunctions.Serialize(objectData)).ToString()
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

        private String iOSMessage(String message, String subject, Int64 eventId, Int32? matchid, String leaderboard)
        {
            String data = String.Empty;

            try
            {
                var structure = new
                {
                    alert = message,
                    subject = subject,
                    @event = eventId,
                    @matchId = matchid,
                    @leaderboard = leaderboard,
                    sound = "cat.caf"
                };

                var objectData = new
                {
                    aps = structure
                };

                var structMessage = new
                {
                    @default = message,
                    APNS_SANDBOX = (GenericFunctions.Serialize(objectData)).ToString(),
                    APNS = (GenericFunctions.Serialize(objectData)).ToString()
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
