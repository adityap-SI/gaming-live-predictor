using Bodog.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace Bodog.Predictor.DataAccess.DataPopulation
{
    public class Populate : Common.BaseDataAccess
    {
        public Populate(IPostgre postgre) : base(postgre)
        {
        }

        #region " INSERT "

        public Int32 SaveTournament(int[] arr_tour_id, string[] arr_tour_name, string[] arr_tour_display_name, DateTime[] arr_tour_start_date, DateTime[] arr_tour_end_date)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_admin_tournament_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = 1;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_tournamentid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = arr_tour_id;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_tournament_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = arr_tour_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_tournament_disp_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = arr_tour_display_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_start_date", NpgsqlDbType.Array | NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Input }).Value = arr_tour_start_date;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_end_date", NpgsqlDbType.Array | NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Input }).Value = arr_tour_end_date;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_is_active", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = 1;
                        NpgsqlParameter value = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(value);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        retVal = value.Value != null && value.Value.ToString().Trim() != "" ? Int32.Parse(value.Value.ToString()) : retVal;
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

        public Int32 SaveSeries(int[] arr_tour_id, int[] arr_series_id, string[] arr_series_name, string[] arr_series_display_name, DateTime[] arr_series_start_date, DateTime[] arr_series_end_date, List<int> arr_comp_type_id)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_admin_series_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = 1;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_tournamentid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = arr_tour_id;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_seriesid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = arr_series_id;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_series_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = arr_series_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_series_disp_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = arr_series_display_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_start_date", NpgsqlDbType.Array | NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Input }).Value = arr_series_start_date;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_end_date", NpgsqlDbType.Array | NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Input }).Value = arr_series_end_date;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_comp_type", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = arr_comp_type_id.ToArray();
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_is_active", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = 1;
                        NpgsqlParameter value = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(value);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        retVal = value.Value != null && value.Value.ToString().Trim() != "" ? Int32.Parse(value.Value.ToString()) : retVal;
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

        public Int32 SaveFixtures(int tournamentId, int seriesId, int[] array_matchid, int[] array_home_teamid, string[] array_series_home_team_name, string[] array_series_home_team_short, int[] array_away_teamid, string[] array_series_away_team_name,
            string[] array_series_away_team_short, DateTime[] array_match_date, string[] array_match_name, string[] array_matchType, string[] array_matchtime_local, string[] array_matchtime_ist, string[] array_matchtime_gmt, string[] array_matchStatus,
            string[] array_matchResult, string[] array_matchFile, string[] array_matchNum, string[] array_venue)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_admin_match_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = 1;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_language_code", NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = "en";
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_tournamentid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tournamentId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_seriesid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = seriesId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = array_matchid;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_home_teamid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = array_home_teamid;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_home_team_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_series_home_team_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_home_team_short_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_series_home_team_short;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_away_teamid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = array_away_teamid;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_away_team_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_series_away_team_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_away_team_short_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_series_away_team_short;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchdate", NpgsqlDbType.Array | NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Input }).Value = array_match_date;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_comp_type", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = array_home_teamid;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_match_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_match_name;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchfile", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchFile;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchnumber", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchNum;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchresult", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchResult;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchstatus", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchStatus;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchtime_gmt", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchtime_gmt;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchtime_ist", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchtime_ist;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchtime_local", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchtime_local;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_matchtype", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_matchType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_venue", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_venue;

                        NpgsqlParameter value = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(value);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        retVal = value.Value != null && value.Value.ToString().Trim() != "" ? Int32.Parse(value.Value.ToString()) : retVal;
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

        public Int32 SaveTeams(int tournamentId, int seriesId, int[] array_teamid, string[] array_team_name, string[] array_team_short, String langCode)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_admin_team_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = 1;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_language_code", NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = langCode;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_tournamentid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tournamentId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_seriesid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = seriesId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_teamid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = array_teamid;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_team_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_team_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_array_team_short_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_team_short;
                        NpgsqlParameter value = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(value);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        retVal = value.Value != null && value.Value.ToString().Trim() != "" ? Int32.Parse(value.Value.ToString()) : retVal;
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

        public Int32 SavePlayersPerTeam(int optType, String langCode, int tournamentId, int seriesId, int team_id, int[] array_playerid, string[] array_player_name, string[] array_display_name, string[] array_display_skill, List<int> arr_skill_id)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_admin_ins_player";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_language_code", NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = langCode;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_tournamentid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tournamentId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_teamid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = team_id;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_seriesid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = seriesId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_playerid", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = array_playerid;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_player_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_player_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_player_display_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_display_name;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_skill_name", NpgsqlDbType.Array | NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = array_display_skill;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_skill_id", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = arr_skill_id.ToArray();

                        NpgsqlParameter value = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCmd.Parameters.Add(value);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCmd.ExecuteScalar();

                        retVal = value.Value != null && value.Value.ToString().Trim() != "" ? Int32.Parse(value.Value.ToString()) : retVal;
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


        #endregion " INSERT "
    }
}