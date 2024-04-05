using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Leaderboard;
using ICC.Predictor.Contracts.Notification;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace ICC.Predictor.DataAccess.Notification
{
    public class Update : Common.BaseDataAccess
    {
        public Update(IPostgre postgre) : base(postgre)
        {
        }

        #region " Windows Service "

        public long Insert(long optType, long tourId, long matchday, long gamedayId)
        {
            long retVal = -50;
            string spName = string.Empty;

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

                        object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() != "" ? long.Parse(value.ToString()) : retVal;
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

        public long UpdateStatus(long optType, long tourId, long notificationId)
        {
            long retVal = -50;
            string spName = string.Empty;

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

                        object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() != "" ? long.Parse(value.ToString()) : retVal;
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
