using ICC.Predictor.Blanket.Scoring;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICC.Predictor.Admin.Models
{
    public class MatchAnswersModel
    {
        public PreMatchQuestions mPreMatchQuestions { get; set; }
        public InningQuestions mFirstInnningQuestions { get; set; }
        public InningQuestions mSecondInnningQuestions { get; set; }

        public int MatchId { get; set; }
        public string MatchFile { get; set; }
    }

    public class MatchAnswersWorker
    {
        MatchFeed mMatchFeed = new MatchFeed();
        List<Lineups> mMatchLineups = new List<Lineups>();
        XDocument mMatchAnalyticsDoc = new XDocument();
        MatchPlayerStats mMatchPlayerStats = new MatchPlayerStats();
        List<Questions> mQuestionsList = new List<Questions>();
        List<Match> mMatches = new List<Match>();
        Match mMatch = new Match();
        ResponseObject responseObject = new ResponseObject();
        HTTPMeta hTTPMeta = new HTTPMeta();
        int OptType = 1;

        public MatchAnswersModel GetModel(Blanket.BackgroundServices.GameLocking _GameLocking, PlayerStatistics _PlayerStatistics, int matchId, string vMatchFile)
        {
            MatchAnswersModel model = new MatchAnswersModel();

            mMatchFeed = _GameLocking.GetMatchScoresFeed(vMatchFile);
            mMatchLineups = _GameLocking.GetLineupsFromMatchFeed(mMatchFeed);

            mMatchAnalyticsDoc = _GameLocking.GetMatchAnalyticsFeed(vMatchFile);
            mMatchPlayerStats = _PlayerStatistics.GetPlayerStats(mMatchFeed, mMatchLineups);
            mMatches = _GameLocking.GetFixturesFeed();
            mMatch = mMatches.Where(c => c.match_Id == matchId.ToString()).FirstOrDefault();

            PreMatchQuestions mPreMatchQuestions = new PreMatchQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, mMatch);
            InningQuestions mFirstInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "First");
            InningQuestions mSecondInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "Second");

            model.mPreMatchQuestions = mPreMatchQuestions;
            model.mFirstInnningQuestions = mFirstInnningQuestions;
            model.mSecondInnningQuestions = mSecondInnningQuestions;

            return model;
        }
    }
}
