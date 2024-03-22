using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Admin.Models
{
    public class DataPopulationModel
    {
        public String League { get; set; }
        public Int32 TournamentId { get; set; }
        public Int32 SeriesId { get; set; }
        public List<Tournament> Tournament { get; set; }
        public List<Series> Series { get; set; }
    }

    #region " Children "

    public class Tournament
    {
        public String Id { get; set; }
        public String Name { get; set; }
    }

    public class Series
    {
        public String Id { get; set; }
        public String Name { get; set; }
    }

    #endregion

    #region " Worker "

    public class DataPopulationWorker
    {
        public DataPopulationModel GetModel(Blanket.Management.Tour tourContext, Blanket.Management.Series seriesContext, Int32 tournament, Int32 series)
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
