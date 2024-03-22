using Bodog.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bodog.Predictor.DataAccess.Analytics
{
    public class Analytics : Common.BaseDataAccess
    {
        public Analytics(IPostgre postgre) : base(postgre)
        {
        }

        public DataSet GetAnalytics(Int32 optType, Int32 tourId)
        {
            String spName = String.Empty;
            DataSet mDSet = new DataSet();
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_user_analytics_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_tot_user_counts", "p_cur_tot_user_counts_daywise", "p_cur_matchday_wise_count", "p_cur_phase_wise_count", "p_cur_match_stats", "p_cur_phase_stats" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_tot_user_counts", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_tot_user_counts_daywise", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_matchday_wise_count", NpgsqlDbType.Refcursor)).Value = cursors[2];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_phase_wise_count", NpgsqlDbType.Refcursor)).Value = cursors[3];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_stats", NpgsqlDbType.Refcursor)).Value = cursors[4];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_phase_stats", NpgsqlDbType.Refcursor)).Value = cursors[5];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        mDSet = DataInitializer.Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return mDSet;
        }

        public DataSet GetUserDetailsReport(Int32 optType)
        {
            String spName = String.Empty;
            DataSet mDSet = new DataSet();
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_cwc_user_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_user" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_user", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        mDSet = DataInitializer.Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return mDSet;
        }
    }
}
