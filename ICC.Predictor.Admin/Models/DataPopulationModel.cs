using ICC.Predictor.Blanket.Management;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Admin.Models
{
    public class DataPopulationModel
    {
        public string League { get; set; }
        public int TournamentId { get; set; }
        public int SeriesId { get; set; }
        public List<Tournament> Tournament { get; set; }
        public List<Series> Series { get; set; }
    }

    #region " Children "

    public class Tournament
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Series
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    #endregion

    #region " Worker "

    public class DataPopulationWorker
    {
        public DataPopulationModel GetModel(Tour tourContext, Blanket.Management.Series seriesContext, int tournament, int series)
        {
            DataPopulationModel model = new DataPopulationModel();

            DataTable dt = tourContext.GetTournaments();

            model.TournamentId = tournament;
            model.Tournament = dt.AsEnumerable().Select(o => new Tournament { Id = o["cf_tournamentid"].ToString(), Name = o["tournament_name"].ToString() }).ToList();
            model.Tournament.Insert(0, new Tournament() { Id = "0", Name = "[ - Tournament - ]" });

            model.SeriesId = series;
            model.Series = new List<Series>() { new Series { Id = "0", Name = "[ - Series - ]" } };

            if (tournament != 0)
            {
                dt = seriesContext.GetSeries(tournament);

                model.SeriesId = series;
                model.Series = dt.AsEnumerable().Select(o => new Series { Id = o["cf_seriesid"].ToString(), Name = o["series_name"].ToString() }).ToList();
                model.Series.Insert(0, new Series() { Id = "0", Name = "[ - Series - ]" });
            }

            return model;
        }




    }

    #endregion
}
