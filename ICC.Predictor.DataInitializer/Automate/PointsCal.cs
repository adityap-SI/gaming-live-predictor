using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Contracts.Automate;
using ICC.Predictor.DataInitializer.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ICC.Predictor.DataInitializer.Automate
{
    public class PointsCal
    {
        public static Matchdays Matchdays(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            Matchdays matchdays = new Matchdays();
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
                            matchdays = (from a in ds.Tables[0].AsEnumerable()
                                         select new Matchdays
                                         {
                                             GamedayId = Convert.IsDBNull(a["cf_tour_gamedayid"]) ? 0 : Convert.ToInt32(a["cf_tour_gamedayid"]),
                                             PhaseId = Convert.IsDBNull(a["cf_phaseid"]) ? 0 : Convert.ToInt32(a["cf_phaseid"]),
                                             Matchday = Convert.IsDBNull(a["cf_match_day"]) ? 0 : Convert.ToInt32(a["cf_match_day"])
                                         }).FirstOrDefault();
                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            matchdays.TeamIds = ds.Tables[1].AsEnumerable().Select(o =>
                            Convert.IsDBNull(o["cf_soccer_teamid"]) ? 0 : Convert.ToInt32(o["cf_soccer_teamid"].ToString())).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataInitializer.Automate.PointsCal.Matchdays: " + ex.Message);
            }

            return matchdays;
        }
    }
}
