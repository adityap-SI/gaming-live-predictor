using ICC.Predictor.Contracts.Leaderboard;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.DataAccess.Common;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace ICC.Predictor.DataAccess.Leaderboard
{
    public class Leaderbaord : BaseDataAccess
    {
        public Leaderbaord(IPostgre postgre) : base(postgre)
        {
        }


        public ResponseObject UserRank(int optType, int tourId, int userId, int teamId, int gamedayId, int phaseId, ref HTTPMeta httpMeta)
        {
            ResponseObject ranks = new ResponseObject();
            int retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_rank_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_cur_rank" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = teamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = phaseId;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_rank", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        ranks = DataInitializer.Leaderboard.Leaderboard.InitializeUserRank(mNpgsqlCmd, cursors);

                        transaction.Commit();

                        retVal = 1;

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
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

            return ranks;
        }

        public ResponseObject Top(int optType, int phaseId, int gamedayId, int pageNo, int top, int tourId, int fromRowNo, int toRowNo, ref HTTPMeta httpMeta)
        {
            ResponseObject vLeaderboard = new ResponseObject();
            long retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_rank_top_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_cur_top_rank", "p_cur_detail" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_page_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = pageNo;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_top_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = top;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = phaseId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_from_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = fromRowNo;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_to_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = toRowNo;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_top_rank", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_detail", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        vLeaderboard = DataInitializer.Leaderboard.Leaderboard.InitializeTop(mNpgsqlCmd, cursors, out retVal);
                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);

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

            return vLeaderboard;
        }

        public Reports AdminLeaderBoard(int optType, int pageno, int topno, int tourId, int phaseId, int gamedayId, int fromrowno, int torowno, ref HTTPMeta httpMeta)
        {
            Reports leaderboard = new Reports();
            int retVal = -50;
            string spName = string.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_user_rank_top_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_cur_top_rank", "p_cur_detail " };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_page_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = pageno;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_top_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = topno;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phaseid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = phaseId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_from_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = fromrowno;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_to_row_number", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = torowno;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_top_rank", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_detail", NpgsqlDbType.Refcursor)).Value = cursors[1];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        leaderboard = DataInitializer.Leaderboard.Leaderboard.InitializeAdminLeaderboard(mNpgsqlCmd, cursors);

                        transaction.Commit();

                        retVal = 1;

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
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

            return leaderboard;
        }
    }
}
