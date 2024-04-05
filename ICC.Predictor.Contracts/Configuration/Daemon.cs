using System;

namespace ICC.Predictor.Contracts.Configuration
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
        public int NotificationDelaySeconds { get; set; }
    }

    public class GameLocking
    {
        public int MatchLockMinutes { get; set; }
        public int IntervalSeconds { get; set; }
        public double LockFirstInningAfter { get; set; }
        public double LockSecondInningAfter { get; set; }
        public int MatchLockNotificationMinutesBefore { get; set; }
        public int SubmitLineupsMinutesBefore { get; set; }
    }

    public class Interval
    {
        public int IntervalMinutes { get; set; }
    }

    public class PointsCalculation
    {
        public int IntervalMinutes { get; set; }
        public string LeaderBoardType { get; set; }
    }

    public class Notification
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
    }

}
