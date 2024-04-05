using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.DataInitializer.Common;
using ICC.Predictor.Library.Utility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace ICC.Predictor.DataInitializer.Notification
{
    public class Notification
    {
        public NotificationDetails Subscriptions(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            NotificationDetails notification = new NotificationDetails();
            DataSet ds = null;

            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            notification = (from a in ds.Tables[0].AsEnumerable()
                                            select new NotificationDetails
                                            {
                                                PlatformEndpoint = Convert.IsDBNull(a["platform_endpoint"]) ? "" : a["platform_endpoint"].ToString(),
                                                SubscriptionARN = Convert.IsDBNull(a["user_subcription_arn"]) ? "" : a["user_subcription_arn"].ToString(),
                                                RetType = Convert.IsDBNull(a["ret_type"]) ? 0 : Convert.ToInt32(a["ret_type"].ToString())
                                            }).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Notification.Notification.Subscriptions: " + ex.Message);
            }

            return notification;
        }

        public DeviceUpdate DeviceUpdate(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            DeviceUpdate device = new DeviceUpdate();
            DataSet ds = null;

            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            device.toSubscribe = (from a in ds.Tables[0].AsEnumerable()
                                                  select new EventDetails
                                                  {
                                                      EventId = Convert.IsDBNull(a["cf_eventid"]) ? 0 : Convert.ToInt32(a["cf_eventid"]),
                                                      IsActive = Convert.IsDBNull(a["is_active"]) ? 0 : Convert.ToInt32(a["is_active"]),
                                                      Language = Convert.IsDBNull(a["uf_language_code"]) ? "" : a["uf_language_code"].ToString(),
                                                      PlatformId = Convert.IsDBNull(a["cf_platformid"]) ? 0 : Convert.ToInt32(a["cf_platformid"])
                                                  }).ToList();
                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            device.toUnsubscribe = (from a in ds.Tables[1].AsEnumerable()
                                                    select new NotificationDetails
                                                    {
                                                        PlatformEndpoint = Convert.IsDBNull(a["platform_endpoint"]) ? "" : a["platform_endpoint"].ToString(),
                                                        SubscriptionARN = Convert.IsDBNull(a["user_subcription_arn"]) ? "" : a["user_subcription_arn"].ToString()
                                                    }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Notification.Notification.DeviceUpdate: " + ex.Message);
            }

            return device;
        }

        public List<Events> EventsGet(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            List<Events> events = new List<Events>();
            DataSet ds = null;

            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            events = (from a in ds.Tables[0].AsEnumerable()
                                      select new Events
                                      {
                                          DeviceId = Convert.IsDBNull(a["deviceid"]) ? "" : a["deviceid"].ToString(),
                                          TeamId = Convert.IsDBNull(a["cf_user_tour_teamid"]) ? 0 : Convert.ToInt32(a["cf_user_tour_teamid"]),
                                          EventId = Convert.IsDBNull(a["cf_eventid"]) ? 0 : Convert.ToInt32(a["cf_eventid"]),
                                          IsActive = Convert.IsDBNull(a["is_active"]) ? 0 : Convert.ToInt32(a["is_active"]),
                                          Language = Convert.IsDBNull(a["language_code"]) ? "" : a["language_code"].ToString(),
                                          PlatformEndpoint = Convert.IsDBNull(a["platform_endpoint"]) ? "" : a["platform_endpoint"].ToString(),
                                          DeviceToken = Convert.IsDBNull(a["device_token"]) ? "" : a["device_token"].ToString()
                                      }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Notification.Notification.EventsGet: " + ex.Message);
            }

            return events;
        }

        public ResponseObject UniqueEvents(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            ResponseObject res = new ResponseObject();
            List<Topics> topics = new List<Topics>();
            DataSet ds = null;

            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            topics = (from a in ds.Tables[0].AsEnumerable()
                                      select new Topics
                                      {
                                          EventId = Convert.IsDBNull(a["cf_eventid"]) ? 0 : Convert.ToInt32(a["cf_eventid"]),
                                          //EventName = Convert.IsDBNull(a["EVENT_NAME"]) ? "" : a["EVENT_NAME"].ToString(),
                                          //EventDesc = Convert.IsDBNull(a["EVENT_DESC"]) ? "" : a["EVENT_DESC"].ToString()
                                      }).ToList();
                        }

                        res.Value = topics;
                        res.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Notification.Notification.UniqueEvents: " + ex.Message);
            }

            return res;
        }

        #region " Internal "

        public ResponseObject TopicsGet(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            ResponseObject res = new ResponseObject();
            List<Topics> topics = new List<Topics>();
            DataSet ds = null;

            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            topics = (from a in ds.Tables[0].AsEnumerable()
                                      select new Topics
                                      {
                                          EventId = Convert.IsDBNull(a["event_id"]) ? 0 : Convert.ToInt32(a["event_id"]),
                                          PlatformId = Convert.IsDBNull(a["platform_id"]) ? 0 : Convert.ToInt32(a["platform_id"]),
                                          Language = Convert.IsDBNull(a["language_code"]) ? "" : a["language_code"].ToString(),
                                          EventTopicARN = Convert.IsDBNull(a["topic_arn"]) ? "" : a["topic_arn"].ToString(),
                                          EventDesc = Convert.IsDBNull(a["description"]) ? "" : a["description"].ToString(),
                                          EventName = Convert.IsDBNull(a["topic_name"]) ? "" : a["topic_name"].ToString()
                                      }).ToList();
                        }

                        res.Value = topics;
                        res.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Notification.Notification.TopicsGet: " + ex.Message);
            }

            return res;
        }

        #endregion
    }
}
