using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Bodog.Predictor.DataAccess.Feeds
{
    public class Gameplay : Common.BaseDataAccess
    {
        public Gameplay(IPostgre postgre) : base(postgre)
        {
        }

        #region " GET "

        public ResponseObject GetFixtures(Int32 optType, Int32 tourId, String langCode, ref HTTPMeta httpMeta)
        {
            ResponseObject fixtures = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            //spName = "cf_match_fixture_get";
            spName = "cf_fant_match_fixture_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    //List<String> cursors = new List<String>() { "p_fixture_cursor", "p_composition_cursor", "p_skill_cursor" };
                    List<String> cursors = new List<String>() { "p_fixture_cursor" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_language_code", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = langCode;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_fixture_cursor", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        //mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_composition_cursor", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        //mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_skill_cursor", NpgsqlDbType.Refcursor)).Value = cursors[2];
                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        fixtures = DataInitializer.Feeds.Gameplay.InitializeFixtures(mNpgsqlCmd, cursors);

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

            return fixtures;
        }

        public ResponseObject GetSkills(Int32 optType, String lang, ref HTTPMeta httpMeta)
        {
            ResponseObject skills = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_admin_skill_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_tour" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_language_code", NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = lang;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_tour", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        skills = DataInitializer.Feeds.Gameplay.InitializeSkills(mNpgsqlCmd, cursors);

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

            return skills;
        }

        public ResponseObject GetQuestions(Int32 optType, Int32 tourId, Int32? QuestionsMatchID, ref HTTPMeta httpMeta)
        {
            ResponseObject Questions = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_question_match_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_question" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = QuestionsMatchID;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_question", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        Questions = DataInitializer.Feeds.Gameplay.InitializeQuestions(mNpgsqlCmd, cursors);

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

            return Questions;
        }

        public ResponseObject GetPredictions(Int32 OptType, Int32 TourId, Int32 UserID, Int32 UserTourTeamId, Int32 MatchId, Int32 TourGamedayId, ref HTTPMeta httpMeta)
        {
            ResponseObject predictions = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_prediction_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<string>() { "p_cur_question", "p_cur_stats" };

                    using (NpgsqlCommand command = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = OptType;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserID;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserTourTeamId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = MatchId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourGamedayId;
                        command.Parameters.Add(new NpgsqlParameter("p_cur_question", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        command.Parameters.Add(new NpgsqlParameter("p_cur_stats", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        command.CommandTimeout = 0;
                        if (connection.State == ConnectionState.Closed) connection.Open();

                        transaction = connection.BeginTransaction();
                        command.ExecuteNonQuery();

                        predictions = DataInitializer.Feeds.Gameplay.InitializeGetPredictions(command, cursors);

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
            return predictions;
        }

        public ResponseObject GetRecentResults(Int32 OptType, Int32 TourId, ref HTTPMeta httpMeta)
        {
            ResponseObject RecentResults = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;
            spName = "cf_fant_team_stats_get";
            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<string>() { "p_team_cursor" };
                    using (NpgsqlCommand command = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = OptType;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourId;
                        command.Parameters.Add(new NpgsqlParameter("p_team_cursor", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        command.CommandTimeout = 0;
                        if (connection.State == ConnectionState.Closed) connection.Open();

                        transaction = connection.BeginTransaction();
                        command.ExecuteNonQuery();

                        RecentResults = DataInitializer.Feeds.Gameplay.InitializeGetRecentResults(command, cursors);
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
            return RecentResults;
        }

        public ResponseObject GetMatchInningStatus(Int32 optType, Int32 tourId, Int32 MatchID, ref HTTPMeta httpMeta)
        {
            ResponseObject InningStatuses = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_match_inning_status_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_cur_status" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = MatchID;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cur_status", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        mNpgsqlCmd.CommandTimeout = 0;
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        InningStatuses = DataInitializer.Feeds.Gameplay.InitializeGetMatchInningStatus(mNpgsqlCmd, cursors);

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

            return InningStatuses;
        }

        public ResponseObject GetUserProfile(Int32 OptType, Int32 TourId, Int32 UserID, Int32 UserTourTeamId, Int32 PlatformId, ref HTTPMeta httpMeta)
        {
            ResponseObject userProfile = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_profile_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<string>() { "p_out_user_stats", "p_out_user_cur" };

                    using (NpgsqlCommand command = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = OptType;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserID;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserTourTeamId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_platformid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = PlatformId;
                        command.Parameters.Add(new NpgsqlParameter("p_out_user_stats", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        command.Parameters.Add(new NpgsqlParameter("p_out_user_cur", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        command.CommandTimeout = 0;
                        if (connection.State == ConnectionState.Closed) connection.Open();

                        transaction = connection.BeginTransaction();
                        command.ExecuteNonQuery();

                        userProfile = DataInitializer.Feeds.Gameplay.InitializeGetUserProfile(command, cursors);

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
            return userProfile;
        }

        public ResponseObject GetOtherUserPredictions(Int32 OptType, Int32 TourId, Int32 UserID, Int32 UserTourTeamId, Int32 MatchId, Int32 TourGamedayId, ref HTTPMeta httpMeta)
        {
            ResponseObject predictions = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_prediction_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<string>() { "p_cur_question", "p_cur_stats" };

                    using (NpgsqlCommand command = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = OptType;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserID;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserTourTeamId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = MatchId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourGamedayId;
                        command.Parameters.Add(new NpgsqlParameter("p_cur_question", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        command.Parameters.Add(new NpgsqlParameter("p_cur_stats", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        command.CommandTimeout = 0;
                        if (connection.State == ConnectionState.Closed) connection.Open();

                        transaction = connection.BeginTransaction();
                        command.ExecuteNonQuery();

                        predictions = DataInitializer.Feeds.Gameplay.InitializeGetPredictions(command, cursors);

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
            return predictions;
        }

        public ResponseObject GetGamePlays(Int32 OptType, Int32 TourId, Int32 UserTourTeamId, ref HTTPMeta httpMeta)
        {
            ResponseObject predictions = new ResponseObject();
            NpgsqlTransaction transaction = null;
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_gameday_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<string>() { "p_cur_gameday" };

                    using (NpgsqlCommand command = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = OptType;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = TourId;
                        command.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = UserTourTeamId;
                        command.Parameters.Add(new NpgsqlParameter("p_cur_gameday", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        command.CommandTimeout = 0;
                        if (connection.State == ConnectionState.Closed) connection.Open();

                        transaction = connection.BeginTransaction();
                        command.ExecuteNonQuery();

                        predictions = DataInitializer.Feeds.Gameplay.InitializeGameplays(command, cursors);

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
            return predictions;
        }

        #endregion " GET "

        #region " POST "

        public ResponseObject UserPrediction(Int32 optType, Int32 tourId, Int32 userId, Int32 userTourTeamId, Int32 matchId, Int32 tourGamedayId,
            Int32 questionId, Int32 optionId, Int32 PlatformId, ref HTTPMeta httpMeta)
        {
            ResponseObject response = new ResponseObject();
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_prediction_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_user_tour_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = userTourTeamId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourGamedayId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_questionid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = questionId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_optionid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optionId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_platformid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = PlatformId;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCommand.Parameters.Add(returnValue);
                        mNpgsqlCommand.CommandTimeout = 0;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        Object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;

                        response.Value = retVal;
                        response.FeedTime = GenericFunctions.GetFeedTime();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
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
            return response;
        }

        #endregion " POST "
    }
}