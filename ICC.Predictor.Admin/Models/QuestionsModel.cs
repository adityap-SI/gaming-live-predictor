using ICC.Predictor.Blanket.Simulation;
using ICC.Predictor.Contracts.Feeds;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Admin.Models
{
    public class QuestionsModel
    {
        public List<MatchQuestions> matchQuestions { get; set; }
        public string QuestionStatus { get; set; }
        public int MatchId { get; set; }
        public string QuestionType { get; set; }
        public Dictionary<string, string> QuestionTypeFilter { get; set; }
        public Dictionary<int, string> QuestionStatusFilter { get; set; }
        public string Header { get; set; }
        public string NotificationText { get; set; }
        public int AbandonedMatchId { get; set; }
        public List<MatchControl> Matches { get; set; }
    }

    public class QuestionStatuses
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Schedule
    {
        [DataType(DataType.Date)]
        public DateTime MatchDate { get; set; }
        public string ShortMatchDate { get; set; }
        public List<Fixtures> Fixtures { get; set; }
    }

    #region " WORKER "

    public class QuestionsWorker
    {
        public QuestionsModel GetModel(Simulation simulationContext)
        {
            QuestionsModel model = new QuestionsModel();
            model.QuestionTypeFilter = new Dictionary<string, string>();

            model.QuestionTypeFilter.Add("PRM", "Pre Match");
            model.QuestionTypeFilter.Add("TEM", "Team");
            model.QuestionTypeFilter.Add("RNG", "Range");
            model.QuestionTypeFilter.Add("QS_PRED", "Predictor");
            model.QuestionTypeFilter.Add("QS_TRIVIA", "Trivia");

            model.QuestionStatusFilter = new Dictionary<int, string>();
            model.QuestionStatusFilter.Add(-2, "All");
            model.QuestionStatusFilter.Add(0, "Unpublished");
            model.QuestionStatusFilter.Add(1, "Published");
            model.QuestionStatusFilter.Add(2, "Locked");
            model.QuestionStatusFilter.Add(3, "Resolved");
            model.QuestionStatusFilter.Add(-1, "Delete");
            model.QuestionStatusFilter.Add(-3, "Notification");
            model.QuestionStatusFilter.Add(-4, "Points_Calculation");

            List<Fixtures> mFixtures = new List<Fixtures>();
            mFixtures = simulationContext.getFixtures();

            model.Matches = mFixtures.Where(a => a.MatchStatus == 1 || a.MatchStatus == 2).Select(o => new MatchControl()
            {
                Id = o.MatchId.ToString(),
                MatchName = o.MatchId.ToString() + "-" + o.TeamAShortName + " vs " + o.TeamBShortName,
                GamedayId = o.GamedayId.ToString(),
                MatchFile = o.Matchfile
            }).ToList();

            return model;
        }
    }

    #endregion " WORKER "
}
