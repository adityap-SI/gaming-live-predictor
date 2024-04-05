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
    public class PeriodicUpdate : Common.BaseDataAccess
    {
        public PeriodicUpdate(IPostgre postgre) : base(postgre)
        {
        }

        public int PartitionUpdate(int optType, int tourId, int gamedayId)
        {
            int retVal = -50;
            string spName = string.Empty;

            spName = "cf_fant_tour_team_partition_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tour_gamedayid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = gamedayId;
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
