using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;


namespace ICC.Predictor.DataAccess.Simulation
{
    public class Simulation : Common.BaseDataAccess
    {
        public Simulation(IPostgre postgre) : base(postgre)
        {
        }

        #region " POST "

        public int SubmitMatchForProcess(int optType, int tourId, int matchId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_fant_match_player_lineup_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCommand.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() == "" ? int.Parse(value.ToString()) : retVal;

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

        public int GenerateUser(int optType, int tourId, int userCount)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_fant_user_ins";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_user_cnt", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = userCount;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCommand.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

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

        public int GenerateUserPredictions(int optType, int tourId, int matchId, int optionId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_fant_user_prediction";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_option_id", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optionId;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCommand.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

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

        public int UserPointProcess(int optType, int tourId, int gamedayId, int matchdayId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_user_point_process";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchdayId;
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output };
                        mNpgsqlCommand.Parameters.Add(returnValue);

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() == "" ? int.Parse(value.ToString()) : retVal;

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

        public int MasterDataRollback(int optType, int tourId, int gamedayId, int matchdayId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_rollback_master_data";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchdayId;

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

        public int UserDataRollback(int optType, int tourId, int gamedayId, int matchdayId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_rollback_user_data";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_match_day", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchdayId;

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

        public int UpdateMatchDateTime(int optType, int tourId, int matchId, string matchdatetime)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "dcfsimu.cf_simu_match_time_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_matchid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = matchId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_match_datetime", NpgsqlDbType.Text) { Direction = ParameterDirection.Input }).Value = matchdatetime;

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
        #endregion

    }
}
