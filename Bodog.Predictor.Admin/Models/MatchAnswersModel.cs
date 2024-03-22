using Bodog.Predictor.Blanket.Scoring;
using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bodog.Predictor.Admin.Models
{
    public class MatchAnswersModel
    {
        public PreMatchQuestions mPreMatchQuestions { get; set; }
        public InningQuestions mFirstInnningQuestions { get; set; }
        public InningQuestions mSecondInnningQuestions { get; set; }

        public Int32 MatchId { get; set; }
        public String MatchFile { get; set; }
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
        Int32 OptType = 1;

        public MatchAnswersModel GetModel(Blanket.BackgroundServices.GameLocking _GameLocking, PlayerStatistics _PlayerStatistics, Int32 matchId, String vMatchFile)
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
