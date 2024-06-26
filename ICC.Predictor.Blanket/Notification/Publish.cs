﻿using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Feeds;
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
    public class Publish : Common.BaseBlanket
    {
        private readonly DataAccess.Notification.Update _DBUpdateContext;
        private readonly DataAccess.Notification.Publish _DBPublishContext;
        private readonly Topics _BlanketTopicsContext;
        private readonly SNS _BlanketSNSContext;

        private readonly int _TourId;

        public Publish(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBUpdateContext = new DataAccess.Notification.Update(postgre);
            _DBPublishContext = new DataAccess.Notification.Publish(postgre);
            _BlanketSNSContext = new SNS(appSettings, aws, postgre, redis, cookies, asset);
            _BlanketTopicsContext = new Topics(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public async Task<bool> Messages(bool sendToiOS, bool sendToAndroid, List<NotificationMessages> messages, int? MatchId = null, bool isTest = false, string leaderboard = "")
        {
            bool success = false;

            try
            {
                // List<NotificationMessages> messages = FetchMessageTexts(out notificationId);

                //if (messages != null && messages.Any() && messages[0].EventId != 0)
                if (messages != null && messages.Any())
                {
                    if (sendToiOS)
                        success = await SendPushNotification(NotificationPlatforms.IOS, messages, isTest, MatchId, leaderboard);

                    if (sendToAndroid)
                        success = await SendPushNotification(NotificationPlatforms.Android, messages, isTest, MatchId, leaderboard);

                }
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }

        private List<NotificationMessages> FetchMessageTexts(out long notificationId)
        {
            notificationId = 0;
            List<NotificationMessages> notificationList = new List<NotificationMessages>();
            string lang = "en";

            try
            {

                //Int64 optType = 1;
                //Int64 platformId = 1;//Web

                //Messages message = _DBPublishContext.FetchEvent(optType, _TourId);

                //if (message == null || message.EventId == 0)
                //    return notificationList;

                //notificationId = message.NotificationId;





                //NotificationMessages n = new NotificationMessages();
                //String messageKey = "", subjectKey = "", messageText = "";

                //if (message.EventId == (Int32)NotificationEvents.TransferSubstitution)
                //{
                //    if (message.WindowType.ToLower().Trim() == "y")
                //        messageKey = "notiSubMessage";
                //    else
                //        messageKey = "notiTranMessage";

                //    subjectKey = "notiTranSubject";

                //    DateTime date = DateTime.ParseExact(message.Date, "yyyy-dd-MM HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
                //    messageText = "";
                //}
                //else if (message.EventId == (Int32)NotificationEvents.PointsCalculation)
                //{
                //    messageKey = "notiPtMessage";
                //    subjectKey = "notiPtSubject";

                //    messageText = "";
                //}

                //n.EventId = message.EventId;
                //n.Language = lang;
                //n.Message = messageText;
                //n.Subject = "";

                //notificationList.Add(n);

            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.Notification.Publish.FetchMessageTexts: " + ex.Message);
            }

            return notificationList;
        }

        public async Task<bool> SendPushNotification(NotificationPlatforms platform, List<NotificationMessages> messageList, bool isTest, int? MatchId = null, string leaderboard = "")
        {
            bool success = false;

            try
            {
                foreach (NotificationMessages m in messageList)
                {
                    Contracts.Notification.Topics t = await _BlanketTopicsContext.TopicByFilter(m.EventId, platform, m.Language);

                    if (!isTest)
                        success = await _BlanketSNSContext.PublishToTopic(m.Message, m.Subject, t.EventTopicARN, platform, m.EventId, MatchId, leaderboard);
                    else
                    {
                        //String isWindowsService = "0";

                        //try
                        //{
                        //    isWindowsService = GenericVariables.IsWindowsService;
                        //}
                        //catch { }

                        string messageLog = GenericFunctions.Serialize(m);
                        string topicLog = GenericFunctions.Serialize(t);

                        //if (isWindowsService == "1")
                        //    Library.Write.Log.File("Notification Message payload: " + Environment.NewLine + platform.ToString() + Environment.NewLine + messageLog + Environment.NewLine + topicLog);
                        //else
                        //    Library.Write.Log.Web("Notification Message payload: " + "<br/>" + platform.ToString() + "<br/>" + messageLog + "<br/>" + topicLog);

                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.Notification.Publish.SendPushNotification: " + ex.Message);
            }

            return success;
        }

    }
}
