using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Leaderboard;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bodog.Predictor.DataAccess.Scoring
{
   public class Answers : Common.BaseDataAccess
    {
        public Answers(IPostgre postgre) : base(postgre)
        {
        }

        public Int64 SubmitQuestionAnswer(Int32 optType, Int32 tourId, Int32 matchId, Int32 questionId, List<Option> mOptions)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_question_match_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_questionid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = questionId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_arr_optionid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = mOptions.Select(c => c.OptionId).ToList();
                        //NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        //mNpgsqlCommand.Parameters.Add(returnValue);
                       

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        //  Object value = returnValue.Value;

                        //retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;

                        retVal = 1;                      

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
            }
            return retVal;
        }

        public Int64 QuestionAnswerProcessUpdate(Int32 optType, Int32 tourId, Int32 matchId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_question_answer_process_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        retVal = 1;

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
            }
            return retVal;
        }

        public Int64 SubmitMatchWinTeam(Int32 optType, Int32 tourId, Int32 matchId, Int32 teamId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_match_winn_team_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = teamId;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        retVal = 1;

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
            }
            return retVal;
        }
    }
}
