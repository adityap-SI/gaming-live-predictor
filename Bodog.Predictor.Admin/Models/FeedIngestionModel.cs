using System;

namespace Bodog.Predictor.Admin.Models
{
    public class FeedIngestionModel
    {
        public Int32? PlayerGamedayId { get; set; }
        public Int32? PlayerTeamGamedayId { get; set; }
        public Int32? TeamGamedayId { get; set; }
        public Int32? GamedayId { get; set; }
        public Int32? MatchId { get; set; }

        public Int32? LeaderBoardGamedayId { get; set; }
        public Int32? LeaderBoardPhaseId { get; set; }

        public Int32? ConstraintGamedayId { get; set; }
        public Int32? ConstraintTeamGamedayId { get; set; }

        public Int32? QuestionsMatchID { get; set; }
    }
}