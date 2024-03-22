using System;

namespace Bodog.Predictor.Library.Utility
{
    public class TimeZone
    {
        /*public static DateTime UTCtoCEST(DateTime UtcTime)
        {
            TimeZoneInfo otherTimezone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            return Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, otherTimezone));
        }*/

        public static DateTime UTCtoIST(DateTime UtcTime)
        {
            //TimeZoneInfo otherTimezone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //return Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(UtcTime, otherTimezone));
            return UtcTime.AddHours(5).AddMinutes(30);
        }

        public static String CurrentUTCtime()
        {
            return DateTime.UtcNow.ToString(new System.Globalization.CultureInfo("en-US"));
        }

        /*public static String CurrentCESTtime()
        {
            return UTCtoCEST(DateTime.UtcNow).ToString(new System.Globalization.CultureInfo("en-US"));
        }*/

        public static String CurrentISTtime()
        {
            return UTCtoIST(DateTime.UtcNow).ToString(new System.Globalization.CultureInfo("en-US"));
        }
    }
}