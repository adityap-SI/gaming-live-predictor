using System;

namespace Bodog.Predictor.Library.Utility
{
    public class Notification
    {
        public static String Body_Automate(Contracts.Common.Notification data)
        {
            String body = String.Empty;

            body = "This is a system generated mail from " + data.Service + " windows service running on " + Environment.MachineName.ToUpper() + ".<br/><br/>";
            body += "The service invoked the " + data.Caption + " process.<br/><br/>";
            body += "" + data.Option + "<br/><br/>";
            body += "Thanks.";

            return body;
        }

        public static String Body_Manual(Contracts.Common.Notification data)
        {
            String body = String.Empty;

            body = "This is a system generated mail for manual " + data.Caption + " triggered from Admin.<br/><br/>";
            body += "The trigger invoked the " + data.Caption + " process.<br/><br/>";
            body += "" + data.Option + "<br/><br/>";
            body += "Thanks.";

            return body;
        }
    }
}