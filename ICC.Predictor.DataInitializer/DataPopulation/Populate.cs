using ICC.Predictor.DataInitializer.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace ICC.Predictor.DataInitializer.DataPopulation
{
    public class Populate
    {
        public static DataTable InitializeTournaments(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            DataSet ds = null;
            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DataInitializer.DataPopulation.Populate.InitializeTournaments: " + ex.Message);
            }

            return new DataTable();
        }

        public static DataTable InitializeSeries(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            DataSet ds = null;
            try
            {
                ds = Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DataInitializer.DataPopulation.Populate.InitializeSeries: " + ex.Message);
            }

            return new DataTable();
        }
    }
}