using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;


namespace Bodog.Predictor.DataAccess.Simulation
{
    public class Simulation : Common.BaseDataAccess
    {
        public Simulation(IPostgre postgre) : base(postgre)
        {
        }

        #region " POST "

        public Int32 SubmitMatchForProcess(Int32 optType, Int32 tourId, Int32 matchId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

                        Object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() == "" ? Int32.Parse(value.ToString()) : retVal;

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

        public Int32 GenerateUser(Int32 optType, Int32 tourId, Int32 userCount)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

        public Int32 GenerateUserPredictions(Int32 optType, Int32 tourId, Int32 matchId, Int32 optionId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

        public Int32 UserPointProcess(Int32 optType, Int32 tourId, Int32 gamedayId, Int32 matchdayId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

                        Object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() == "" ? Int32.Parse(value.ToString()) : retVal;

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

        public Int32 MasterDataRollback(Int32 optType, Int32 tourId, Int32 gamedayId, Int32 matchdayId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

        public Int32 UserDataRollback(Int32 optType, Int32 tourId, Int32 gamedayId, Int32 matchdayId)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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

        public Int32 UpdateMatchDateTime(Int32 optType, Int32 tourId, Int32 matchId, String matchdatetime)
        {
            Int32 retVal = -50;
            String spName = String.Empty;

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
