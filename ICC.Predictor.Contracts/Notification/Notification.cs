using System;
using System.Collections.Generic;

namespace ICC.Predictor.Contracts.Notification
{
    public class Subscription
    {
        public string DeviceToken { get; set; }
        public string PlatformEndpoint { get; set; }
        public string SubscriptionArn { get; set; }
        public NotificationPlatforms Platform { get; set; }
        public string DeviceIdentity { get; set; }
        public bool EnableNotification { get; set; }
        public int IsActive { get; set; }
        public int EventId { get; set; }
    }

    public class NotificationDetails
    {
        public string PlatformEndpoint { get; set; }
        public string SubscriptionARN { get; set; }
        public int RetType { get; set; }
    }

    public class EventDetails
    {
        public int EventId { get; set; }
        public int IsActive { get; set; }
        public string Language { get; set; }
        public int PlatformId { get; set; }
    }

    public class DeviceUpdate
    {
        public List<EventDetails> toSubscribe { get; set; }
        public List<NotificationDetails> toUnsubscribe { get; set; }
    }

    public class Events
    {
        public string DeviceId { get; set; }
        public int TeamId { get; set; }
        public int EventId { get; set; }
        public int IsActive { get; set; }
        public string Language { get; set; }
        public string PlatformEndpoint { get; set; }
        public string DeviceToken { get; set; }
    }

    public class Topics
    {
        public int EventId { get; set; }
        public int PlatformId { get; set; }
        public string Language { get; set; }
        public string EventTopicARN { get; set; }
        public string EventDesc { get; set; }
        public string EventName { get; set; }
    }

    public class Messages
    {
        public int EventId { get; set; }
        public int NotificationId { get; set; }
        public string WindowType { get; set; }
        public string Date { get; set; }
    }

    public class NotificationMessages
    {
        public int EventId { get; set; }
        public string Language { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
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
        public int MatchId { get; set; }
        public bool PreMatchNotification { get; set; }
    }
    #endregion
}
