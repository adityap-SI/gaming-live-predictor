using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;

namespace ICC.Predictor.DataAccess.BackgroundServices
{
    public class GameLocking : Common.BaseDataAccess
    {
        public GameLocking(IPostgre postgre) : base(postgre)
        {
        }

        public int Lock(int optType, int tourId, int matchId, int inningNo)
        {
            int retVal = -50;
            string spName = string.Empty;

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

                        object value = returnValue.Value;
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
            }

            return retVal;
        }

        public int UnLock(int optType, int tourId, int matchId, int inningNo)
        {
            int retVal = -50;
            string spName = string.Empty;

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

                        object value = returnValue.Value;
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
            }

            return retVal;
        }

        public int InsertMatchLineups(int optType, int tourId, int matchId, List<Lineups> lineups, List<string> skillName, List<int> skillId)
        {
            int retVal = -50;
            string spName = string.Empty;

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

                        object value = returnValue.Value;
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
            }

            return retVal;
        }

        public int ProcessMatchToss(int optType, int tourId, int matchId, int inningOneBatTeamId, int inningOneBowlTeamId, int inningTwoBatTeamId, int inningTwoBowlTeamId)
        {
            int retVal = -50;
            string spName = string.Empty;

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

                        object value = returnValue.Value;
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
            }

            return retVal;
        }
    }
}
