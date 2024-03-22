using Bodog.Predictor.Contracts.Automate;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Bodog.Predictor.DataAccess.Automate
{
    public class PointsCal : Common.BaseDataAccess
    {
        public PointsCal(IPostgre postgre) : base(postgre)
        {
        }

        public Matchdays Matchdays(Int32 optType, Int32 tourId)
        {
            Matchdays matchdays = new Matchdays();
            NpgsqlTransaction transaction = null;
            String spName = String.Empty;

            spName = "cf_fant_point_process_match_day_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {

                    List<String> cursors = new List<String>() { "p_cur_match_day", "p_cur_match_day_teamid" };

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

        public Int32 UserPointsProcess(Int32 optType, Int32 tourId, Int32 gamedayId, Int32 matchday)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

                        Object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
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

        public DataSet UserPointsProcessReports(Int32 optType, Int32 processRetVal, Int32 tourId, Int32 gamedayId, Int32 matchday)
        {
            DataSet ds = new DataSet();
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "dcfrank.cf_user_point_process_report_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_report" };

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

                        ds = DataInitializer.Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

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
