using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;

namespace Bodog.Predictor.DataAccess.BackgroundServices
{
    public class GameLocking : Common.BaseDataAccess
    {
        public GameLocking(IPostgre postgre) : base(postgre)
        {
        }

        public Int32 Lock(Int32 optType, Int32 tourId, Int32 matchId, Int32 inningNo)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_game_lock";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_inning_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = inningNo;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        Object value = returnValue.Value;
                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
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

        public Int32 UnLock(Int32 optType, Int32 tourId, Int32 matchId, Int32 inningNo)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_game_unlock";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_inning_no", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = inningNo;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        Object value = returnValue.Value;
                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
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

        public Int32 InsertMatchLineups(Int32 optType, Int32 tourId, Int32 matchId, List<Lineups> lineups, List<string> skillName, List<int> skillId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_match_player_lineup_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {                        
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_playerid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = lineups.Select(c => Convert.ToInt32(c.PlayerId)).ToList();
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_player_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = lineups.Select(c => c.PlayerName).ToList();
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_player_display_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = lineups.Select(c => c.PlayerName).ToList();
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_skill_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = skillName;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_skill_id", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = skillId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_arr_teamid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = lineups.Select(c => Convert.ToInt32(c.TeamId)).ToList();
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        Object value = returnValue.Value;
                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
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

        public Int32 ProcessMatchToss(Int32 optType, Int32 tourId, Int32 matchId, Int32 inningOneBatTeamId, Int32 inningOneBowlTeamId, Int32 inningTwoBatTeamId, Int32 inningTwoBowlTeamId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_fant_match_toss_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_inning_1_bat_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = inningOneBatTeamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_inning_1_bwl_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = inningOneBowlTeamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_inning_2_bat_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = inningTwoBatTeamId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_inning_2_bwl_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = inningTwoBowlTeamId;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        Object value = returnValue.Value;
                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;
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
