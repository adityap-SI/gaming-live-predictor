using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Contracts.Automate;
using ICC.Predictor.DataAccess.Common;
using ICC.Predictor.DataInitializer.Common;
using ICC.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace ICC.Predictor.DataAccess.Automate
{
    public class PointsCal : BaseDataAccess
    {
        public PointsCal(IPostgre postgre) : base(postgre)
        {
        }

        public Matchdays Matchdays(int optType, int tourId)
        {
            Matchdays matchdays = new Matchdays();
            NpgsqlTransaction transaction = null;
            string spName = string.Empty;

            spName = "cf_fant_point_process_match_day_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {

                    List<string> cursors = new List<string>() { "p_cur_match_day", "p_cur_match_day_teamid" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_day", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_match_day_teamid", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        matchdays = DataInitializer.Automate.PointsCal.Matchdays(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return matchdays;
        }

        public int UserPointsProcess(int optType, int tourId, int gamedayId, int matchday)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfrank.cf_user_point_process";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchday;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        //because the query takes time.
                        mNpgsqlCmd.CommandTimeout = 36000;

                        object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? int.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DataAccess.Automate.PointsCal.UserPointsProcess: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return retVal;
        }

        public DataSet UserPointsProcessReports(int optType, int processRetVal, int tourId, int gamedayId, int matchday)
        {
            DataSet ds = new DataSet();
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "dcfrank.cf_user_point_process_report_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_cur_report" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_point_process_ret_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = processRetVal;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchday;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_report", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw new Exception("DataAccess.Automate.PointsCal.UserPointsProcessReports: " + ex.Message);
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return ds;
        }
    }
}
