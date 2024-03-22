using System;

namespace Bodog.Predictor.Library.Utility
{
    public static class Extensions
    {
        public static String USFormat(this String date)
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

        public static Int32 SmartIntParse(this String value)
        {

            return Int32.Parse(String.IsNullOrEmpty(value) ? "0" : value);
        }

        public static Double SmartDoubleParse(this String value)
        {

            return Double.Parse(String.IsNullOrEmpty(value) ? "0" : value);
        }


        public static DateTime USFormatDate(this String date)
        {
            return Convert.ToDateTime(date, new System.Globalization.CultureInfo("en-US"));
        }

    }
}