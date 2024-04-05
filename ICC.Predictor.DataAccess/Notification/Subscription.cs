using ICC.Predictor.Contracts.Leaderboard;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace ICC.Predictor.DataAccess.Notification
{
    public class Subscription : Common.BaseDataAccess
    {
        public Subscription(IPostgre postgre) : base(postgre)
        {
        }

        public NotificationDetails Subscriptions(int optType, int tourId, int userId, int teamId, string deviceToken, int platformId, string deviceId,
            int notificationEnabled, string language, string platformEndpoint, string subscriptionArn, int isActive, int eventId, ref HTTPMeta httpMeta)
        {
            NotificationDetails notification = new NotificationDetails();
            long retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_nt_subscription_ins_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_user_details" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = teamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_device_token", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = deviceToken;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_platformid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = platformId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_deviceid", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = BareEncryption.BaseEncrypt(deviceId);
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_is_enable_notification", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = notificationEnabled;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_language_code", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = language;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_platform_endpoint", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = platformEndpoint;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_user_subcription_arn", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = subscriptionArn;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_is_active", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = isActive;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_eventid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = eventId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_user_details", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        //mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        //Object value = mNpgsqlCmd.Parameters["p_ret_type"].Value;
                        //retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;

                        notification = new DataInitializer.Notification.Notification().Subscriptions(mNpgsqlCmd, cursors);

                        retVal = notification.RetType;

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Classes.Notification.Subscription.Subscriptions: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return notification;
        }

        public DeviceUpdate DeviceUpdate(int optType, int tourId, int userId, int teamId, string deviceToken, string platformEndPoint, int platformId,
            string deviceId, ref HTTPMeta httpMeta)
        {
            DeviceUpdate device = new DeviceUpdate();
            long retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_nt_event_details_login_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_user_event_sub_cursor", "p_user_event_unsub_cursor" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = teamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_device_token", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = deviceToken;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_platformid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = platformId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_deviceid", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = BareEncryption.BaseEncrypt(deviceId);

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_user_event_sub_cursor", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_user_event_unsub_cursor", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        retVal = 1;

                        device = new DataInitializer.Notification.Notification().DeviceUpdate(mNpgsqlCmd, cursors);

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Classes.Notification.Subscription.DeviceUpdate: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return device;
        }

        public List<Events> EventsGet(int optType, int teamId, string deviceId, int tourId, ref HTTPMeta httpMeta)
        {
            List<Events> events = new List<Events>();
            long retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_nt_event_details_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_user_event_cursor" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = teamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_deviceid", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = BareEncryption.BaseEncrypt(deviceId);
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_user_event_cursor", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        events = new DataInitializer.Notification.Notification().EventsGet(mNpgsqlCmd, cursors);

                        retVal = 1;

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Classes.Notification.Subscription.EventsGet: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return events;
        }

        public ResponseObject UniqueEvents(int optType, int tourId, ref HTTPMeta httpMeta)
        {
            ResponseObject events = new ResponseObject();
            long retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_nt_event_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_event_details" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_event_details", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        events = new DataInitializer.Notification.Notification().UniqueEvents(mNpgsqlCmd, cursors);

                        retVal = 1;

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Notification.Subscription.UniqueEvents: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return events;
        }

        #region " Internal "

        public ResponseObject TopicsGet(int optType, int tourId, ref HTTPMeta httpMeta)
        {
            ResponseObject topics = new ResponseObject();
            long retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_nt_topic_arn_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_topic_details" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_topic_details", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        topics = new DataInitializer.Notification.Notification().TopicsGet(mNpgsqlCmd, cursors);

                        retVal = 1;

                        transaction.Commit();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Notification.Subscription.TopicsGet: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return topics;
        }

        #endregion
    }
}
