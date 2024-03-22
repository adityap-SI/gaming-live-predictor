using System;
using System.Collections.Generic;

namespace Bodog.Predictor.Contracts.Notification
{
    public class Subscription
    {
        public String DeviceToken { get; set; }
        public String PlatformEndpoint { get; set; }
        public String SubscriptionArn { get; set; }
        public NotificationPlatforms Platform { get; set; }
        public String DeviceIdentity { get; set; }
        public bool EnableNotification { get; set; }
        public Int32 IsActive { get; set; }
        public Int32 EventId { get; set; }
    }

    public class NotificationDetails
    {
        public String PlatformEndpoint { get; set; }
        public String SubscriptionARN { get; set; }
        public Int32 RetType { get; set; }
    }

    public class EventDetails
    {
        public Int32 EventId { get; set; }
        public Int32 IsActive { get; set; }
        public String Language { get; set; }
        public Int32 PlatformId { get; set; }
    }

    public class DeviceUpdate
    {
        public List<EventDetails> toSubscribe { get; set; }
        public List<NotificationDetails> toUnsubscribe { get; set; }
    }

    public class Events
    {
        public String DeviceId { get; set; }
        public Int32 TeamId { get; set; }
        public Int32 EventId { get; set; }
        public Int32 IsActive { get; set; }
        public String Language { get; set; }
        public String PlatformEndpoint { get; set; }
        public String DeviceToken { get; set; }
    }

    public class Topics
    {
        public Int32 EventId { get; set; }
        public Int32 PlatformId { get; set; }
        public String Language { get; set; }
        public String EventTopicARN { get; set; }
        public String EventDesc { get; set; }
        public String EventName { get; set; }
    }

    public class Messages
    {
        public Int32 EventId { get; set; }
        public Int32 NotificationId { get; set; }
        public String WindowType { get; set; }
        public String Date { get; set; }
    }

    public class NotificationMessages
    {
        public Int32 EventId { get; set; }
        public String Language { get; set; }
        public String Subject { get; set; }
        public String Message { get; set; }
    }

    #region " Notification Text "
    public class NotificationText
    {
        public string preMatch { get; set; }
        public string openInningOne { get; set; }
        public string openInningTwo { get; set; }
        public string postMatch { get; set; }
    }
    #endregion

    public enum NotificationPlatforms
    {
        IOS = 2,
        Android = 1
    }

    public enum NotificationEvents { Generic = 1 }

    #region " Pre Match Notification Status "
    public class NotificationStatus
    {
        public Int32 MatchId { get; set; }
        public Boolean PreMatchNotification { get; set; }
    }
    #endregion
}
