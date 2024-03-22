using System;

namespace Bodog.Predictor.Contracts.Configuration
{
    public class Daemon
    {
        public GameLocking GameLocking { get; set; }
        public Notification Notification { get; set; }
        public PointsCalculation PointsCalculation { get; set; }
        public Interval MatchAnswerCalculation { get; set; }
        public Interval PeriodicUpdate { get; set; }
        public Interval PeriodicQuestionsUpdate { get; set; }
        public Interval Analytics { get; set; }
        public Int32 NotificationDelaySeconds { get; set; }
    }

    public class GameLocking
    {
        public Int32 MatchLockMinutes { get; set; }
        public Int32 IntervalSeconds { get; set; }
        public Double LockFirstInningAfter { get; set; }
        public Double LockSecondInningAfter { get; set; }
        public Int32 MatchLockNotificationMinutesBefore { get; set; }
        public Int32 SubmitLineupsMinutesBefore { get; set; }
    }

    public class Interval
    {
        public Int32 IntervalMinutes { get; set; }
    }

    public class PointsCalculation
    {
        public Int32 IntervalMinutes { get; set; }
        public String LeaderBoardType { get; set; }
    }

    public class Notification
    {
        public String Sender { get; set; }
        public String Recipient { get; set; }
    }

}
