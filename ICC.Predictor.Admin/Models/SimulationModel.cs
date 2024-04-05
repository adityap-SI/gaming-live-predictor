using ICC.Predictor.Blanket.Simulation;
using ICC.Predictor.Contracts.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Admin.Models
{
    public class SimulationModel
    {
        public string TourId { get; set; }
        public string MatchId { get; set; }
        //public String MatchProccessMatchId { get; set; }
        public string GenenrateUserPredictionMatchId { get; set; }
        public string SubmitLineUpMatchId { get; set; }
        public string SubmitTossMatchId { get; set; }
        public string LockMatchId { get; set; }
        public string LockInningMatchId { get; set; }
        public string UnlockInningMatchId { get; set; }
        public string SubmitAnswerMatchId { get; set; }
        public string MatchFile { get; set; }
        public string TossMatchFile { get; set; }
        public int? UserCount { get; set; }
        public int? OptionId { get; set; }
        public int? GamedayId { get; set; }
        public int? matchdayId { get; set; }
        public int? InningId { get; set; }
        public int? UnlockInningId { get; set; }
        public string UpdateMatchId { get; set; }
        public string UpdateMatchDateTime { get; set; }
        public List<MatchControl> Matches { get; set; }
        public int? AbandonMatchId { get; set; }
        public List<MatchControl> AbandonMatches { get; set; }
    }
    #region " Children "

    public class MatchControl
    {
        public string Id { get; set; }
        public string GamedayId { get; set; }
        public string MatchName { get; set; }
        public string MatchFile { get; set; }
    }

    #endregion

    public class SimulationWorker
    {
        public SimulationModel GetModel(Simulation simulationContext,
           SimulationModel formModel, bool fetchData = false)
        {

            SimulationModel model = new SimulationModel();
            List<Fixtures> mFixtures = new List<Fixtures>();

            #region " Match Dropdown "

            mFixtures = simulationContext.getFixtures();

            model.Matches = mFixtures.Select(o => new MatchControl()
            {
                Id = o.MatchId.ToString(),
                MatchName = o.MatchId.ToString() + "-" + o.TeamAShortName + " vs " + o.TeamBShortName,
                GamedayId = o.GamedayId.ToString(),
                MatchFile = o.Matchfile
            }).ToList();

            model.AbandonMatches = mFixtures.Where(a => a.MatchStatus == 1 || a.MatchStatus == 2).Select(o => new MatchControl()
            {
                Id = o.MatchId.ToString(),
                MatchName = o.MatchId.ToString() + "-" + o.TeamAShortName + " vs " + o.TeamBShortName,
                GamedayId = o.GamedayId.ToString(),
                MatchFile = o.Matchfile
            }).ToList();

            #endregion

            return model;

        }
    }

}
