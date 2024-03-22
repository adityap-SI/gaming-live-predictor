using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Leaderboard;
using Bodog.Predictor.Contracts.Notification;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Bodog.Predictor.DataAccess.Notification
{
    public class Update : Common.BaseDataAccess
    {
        public Update(IPostgre postgre) : base(postgre)
        {
        }

        #region " Windows Service "

        public Int64 Insert(Int64 optType, Int64 tourId, Int64 matchday, Int64 gamedayId)
        {
            Int64 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_nt_transfer_pointcalculation_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchday;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;

                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        Object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() != "" ? Int64.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DataAccess.Notiication.Update.Insert: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }

        public Int64 UpdateStatus(Int64 optType, Int64 tourId, Int64 notificationId)
        {
            Int64 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_nt_push_notification_status_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_user_notificationid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = notificationId;

                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        Object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() != "" ? Int64.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DataAccess.Notiication.Update.UpdateStatus: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }

        #endregion
    }
}
