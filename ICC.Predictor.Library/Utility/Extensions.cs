using System;

namespace ICC.Predictor.Library.Utility
{
    public static class Extensions
    {
        public static string USFormat(this string date)
        {
            try
            {
                return DateTime.Parse(date).ToString(new System.Globalization.CultureInfo("en-US"));
            }
            catch
            {
                try
                {
                    return DateTime.Parse(date).ToString("MM/dd/yyyy hh:mm tt");
                }
                catch
                {
                    return date;
                }
            }
        }

        public static int SmartIntParse(this string value)
        {

            return int.Parse(string.IsNullOrEmpty(value) ? "0" : value);
        }

        public static double SmartDoubleParse(this string value)
        {

            return double.Parse(string.IsNullOrEmpty(value) ? "0" : value);
        }


        public static DateTime USFormatDate(this string date)
        {
            return Convert.ToDateTime(date, new System.Globalization.CultureInfo("en-US"));
        }

    }
}