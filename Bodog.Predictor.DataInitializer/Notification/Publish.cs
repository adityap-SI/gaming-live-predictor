using Bodog.Predictor.Contracts.Notification;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Bodog.Predictor.DataInitializer.Notification
{
    public class Publish
    {
        public Messages FetchEvent(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            Messages message = new Messages();
            DataSet ds = null;

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            message = (from a in ds.Tables[0].AsEnumerable()
                                       select new Messages
                                       {
                                           EventId = Convert.IsDBNull(a["uf_notification_eventid"]) ? 0 : Convert.ToInt32(a["uf_notification_eventid"]),
                                           NotificationId = Convert.IsDBNull(a["uf_user_notificationid"]) ? 0 : Convert.ToInt32(a["uf_user_notificationid"]),
                                           WindowType = Convert.IsDBNull(a["catg_var_val3"]) ? "" : a["catg_var_val3"].ToString(),
                                           Date = Convert.IsDBNull(a["catg_var_val1"]) ? "" : a["catg_var_val1"].ToString()
                                       }).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Notification.Publish.FetchEvent: " + ex.Message);
            }

            return message;
        }
    }
}
