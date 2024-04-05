using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.DataAccess.Common;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ICC.Predictor.DataAccess.AdminQuestions
{
    public class AdminQuestions : BaseDataAccess
    {
        public AdminQuestions(IPostgre postgre) : base(postgre)
        {
        }

        public int SaveQuestions(int optType, int tourId, int matchId, int questionId, string questionDesc, string questionType, int questionStatus, int[] optionId, string[] optionDesc, int[] isCorrect)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "cf_qdmin_match_question_map";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_questionid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = questionId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_question_dec", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = questionDesc;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_question_type", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = questionType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_question_status", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = questionStatus;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_cf_optionid", NpgsqlDbType.Array | NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optionId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_option_dec", NpgsqlDbType.Array | NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = optionDesc;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_is_correct", NpgsqlDbType.Array | NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = isCorrect;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? int.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }

                return retVal;
            }
        }

        public List<MatchQuestions> GetMatchQuestions(int optType, int tourId, int matchId, ref HTTPMeta httpMeta, ref int retVal)
        {
            List<MatchQuestions> questions = new List<MatchQuestions>();
            NpgsqlTransaction transaction = null;
            retVal = -50;
            string spName = string.Empty;

            spName = "cf_qdmin_question_match_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<string> cursors = new List<string>() { "p_cur_question" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_question", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        questions = DataInitializer.AdminQuestions.AdminQuestions.InitializeQuestionId(mNpgsqlCmd, cursors, ref retVal);

                        transaction.Commit();

                        //retVal = 1;

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

            return questions;
        }

        public int AbandonMatch(int optType, int tourId, int abandonMatchId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "cf_qdmin_abandoned_match_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = abandonMatchId;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        object value = mNpgsqlCmd.ExecuteScalar();

                        retVal = value != null && value.ToString().Trim() != "" ? int.Parse(value.ToString()) : retVal;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }

                return retVal;
            }
        }
    }
}
