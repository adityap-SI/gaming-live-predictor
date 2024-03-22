using Bodog.Predictor.Contracts.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Admin.Models
{
    public class SimulationModel
    {
        public String TourId { get; set; }
        public String MatchId { get; set; }
        //public String MatchProccessMatchId { get; set; }
        public String GenenrateUserPredictionMatchId { get; set; }
        public String SubmitLineUpMatchId { get; set; }
        public String SubmitTossMatchId { get; set; }
        public String LockMatchId { get; set; }
        public String LockInningMatchId { get; set; }
        public String UnlockInningMatchId { get; set; }
        public String SubmitAnswerMatchId { get; set; }
        public String MatchFile { get; set; }
        public String TossMatchFile { get; set; }
        public Int32? UserCount { get; set; }
        public Int32? OptionId { get; set; }
        public Int32? GamedayId { get; set; }
        public Int32? matchdayId { get; set; }
        public Int32? InningId { get; set; }
        public Int32? UnlockInningId { get; set; }
        public String UpdateMatchId { get; set; }
        public String UpdateMatchDateTime { get; set; }
        public List<MatchControl> Matches { get; set; }
        public Int32? AbandonMatchId { get; set; }
        public List<MatchControl> AbandonMatches { get; set; }
    }
    #region " Children "

    public class MatchControl
    {
        public String Id { get; set; }
        public String GamedayId { get; set; }
        public String MatchName { get; set; }
        public String MatchFile { get; set; }
    }

    #endregion

    public class SimulationWorker
    {
        public SimulationModel GetModel(Blanket.Simulation.Simulation simulationContext,
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
