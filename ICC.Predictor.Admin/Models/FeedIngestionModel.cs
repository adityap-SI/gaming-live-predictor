using System;

namespace ICC.Predictor.Admin.Models
{
    public class FeedIngestionModel
    {
        public int? PlayerGamedayId { get; set; }
        public int? PlayerTeamGamedayId { get; set; }
        public int? TeamGamedayId { get; set; }
        public int? GamedayId { get; set; }
        public int? MatchId { get; set; }

        public int? LeaderBoardGamedayId { get; set; }
        public int? LeaderBoardPhaseId { get; set; }

        public int? ConstraintGamedayId { get; set; }
        public int? ConstraintTeamGamedayId { get; set; }

        public int? QuestionsMatchID { get; set; }
    }
}