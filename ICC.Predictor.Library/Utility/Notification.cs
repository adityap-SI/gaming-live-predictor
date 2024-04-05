using System;

namespace ICC.Predictor.Library.Utility
{
    public class Notification
    {
        public static string Body_Automate(Contracts.Common.Notification data)
        {
            string body = string.Empty;

            body = "This is a system generated mail from " + data.Service + " windows service running on " + Environment.MachineName.ToUpper() + ".<br/><br/>";
            body += "The service invoked the " + data.Caption + " process.<br/><br/>";
            body += "" + data.Option + "<br/><br/>";
            body += "Thanks.";

            return body;
        }

        public static string Body_Manual(Contracts.Common.Notification data)
        {
            string body = string.Empty;

            body = "This is a system generated mail for manual " + data.Caption + " triggered from Admin.<br/><br/>";
            body += "The trigger invoked the " + data.Caption + " process.<br/><br/>";
            body += "" + data.Option + "<br/><br/>";
            body += "Thanks.";

            return body;
        }
    }
}