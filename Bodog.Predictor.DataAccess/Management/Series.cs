using Bodog.Predictor.Interfaces.Connection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bodog.Predictor.DataAccess.Management
{
    public class Series : Common.BaseDataAccess
    {
        public Series(IPostgre postgre) : base(postgre)
        {
        }

        public DataTable GetSeries(Int32 optType, Int32 tourId, Int32 tournamentId)
        {
            String spName = String.Empty;
            DataTable dt = new DataTable();
            NpgsqlTransaction transaction = null;

            spName = "cf_admin_tournament_series_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_tournament_series" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_tournamentid", NpgsqlDbType.Integer) { Direction = ParameterDirection.Input }).Value = tournamentId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_tournament_series", NpgsqlDbType.Refcursor)).Value = cursors[0];

                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        dt = DataInitializer.DataPopulation.Populate.InitializeSeries(mNpgsqlCmd, cursors);

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return dt;
        }
    }
}
