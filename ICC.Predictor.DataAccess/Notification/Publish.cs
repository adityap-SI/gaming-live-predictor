using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Leaderboard;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace ICC.Predictor.DataAccess.Notification
{
    public class Publish : Common.BaseDataAccess
    {
        public Publish(IPostgre postgre) : base(postgre)
        {
        }

        public Messages FetchEvent(long optType, long tourId)
        {
            Messages message = new Messages();
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_nt_generic_notification_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_cur_notification" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_notification", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        message = new DataInitializer.Notification.Publish().FetchEvent(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Notification.Publish.FetchEvent: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return message;
        }
    }
}
