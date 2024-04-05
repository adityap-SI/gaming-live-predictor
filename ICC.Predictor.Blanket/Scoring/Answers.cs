using ICC.Predictor.Contracts.Feeds;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.DataAccess.Feeds;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Blanket.Scoring
{
    public class Answers : BaseBlanket
    {
        private readonly BackgroundServices.GameLocking _GameLocking;
        private readonly PlayerStatistics _PlayerStatistics;
        private readonly Gameplay _DBContext;
        private readonly int _TourId;

        public Answers(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
           : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _GameLocking = new BackgroundServices.GameLocking(appSettings, null, aws, postgre, redis, cookies, asset);
            _DBContext = new Gameplay(postgre);
            _PlayerStatistics = new PlayerStatistics(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public List<Questions> GetQuestionsWithAnswers(string vMatchFile, int vMatchId)
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



            mMatchFeed = _GameLocking.GetMatchScoresFeed(vMatchFile);
            mMatchLineups = _GameLocking.GetLineupsFromMatchFeed(mMatchFeed);

            mMatchAnalyticsDoc = _GameLocking.GetMatchAnalyticsFeed(vMatchFile);
            mMatchPlayerStats = _PlayerStatistics.GetPlayerStats(mMatchFeed, mMatchLineups);


            responseObject = _DBContext.GetQuestions(OptType, _TourId, vMatchId, ref hTTPMeta);
            mQuestionsList = GenericFunctions.Deserialize<List<Questions>>(GenericFunctions.Serialize(responseObject.Value))
                .Where(x => x.QuestionOccurrence.ToLower() == "prm").ToList();

            mMatches = _GameLocking.GetFixturesFeed();
            mMatch = mMatches.Where(c => c.match_Id == vMatchId.ToString()).FirstOrDefault();

            PreMatchQuestions mPreMatchQuestions = new PreMatchQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, mMatch);
            //InningQuestions mFirstInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "First");
            //InningQuestions mSecondInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "Second");

            foreach (Questions mQuestions in mQuestionsList)
            {
                //if (mQuestions.QuestionOccurrence == "PRM")
                //{
                GetCorrectOption(mQuestions, mPreMatchQuestions);
                //}
                //else
                //{
                //    if (mQuestions.InningNo == 1)
                //    {
                //        GetCorrectOption(mQuestions, mFirstInnningQuestions);
                //    }
                //    else if (mQuestions.InningNo == 2)
                //    {
                //        GetCorrectOption(mQuestions, mSecondInnningQuestions);
                //    }
                //}

            }

            return mQuestionsList;
        }


        public List<Questions> GetFirstInningQuestionsWithAnswers(string vMatchFile, int vMatchId)
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



            mMatchFeed = _GameLocking.GetMatchScoresFeed(vMatchFile);
            mMatchLineups = _GameLocking.GetLineupsFromMatchFeed(mMatchFeed);

            mMatchAnalyticsDoc = _GameLocking.GetMatchAnalyticsFeed(vMatchFile);
            mMatchPlayerStats = _PlayerStatistics.GetPlayerStats(mMatchFeed, mMatchLineups);


            responseObject = _DBContext.GetQuestions(OptType, _TourId, vMatchId, ref hTTPMeta);
            mQuestionsList = GenericFunctions.Deserialize<List<Questions>>(GenericFunctions.Serialize(responseObject.Value)).Where(x => x.QuestionOccurrence.ToLower() == "ing1").Select(y => y).ToList();

            mMatches = _GameLocking.GetFixturesFeed();
            mMatch = mMatches.Where(c => c.match_Id == vMatchId.ToString()).FirstOrDefault();

            //PreMatchQuestions mPreMatchQuestions = new PreMatchQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, mMatch);
            InningQuestions mFirstInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "First");
            //InningQuestions mSecondInnningQuestions = new InningQuestions(mMatchFeed, mMatchPlayerStats, mMatchAnalyticsDoc, "Second");

            foreach (Questions mQuestions in mQuestionsList)
            {
                //if (mQuestions.QuestionOccurrence == "PRM")
                //{
                //    GetCorrectOption(mQuestions, mPreMatchQuestions);
                //}
                //else
                //{
                if (mQuestions.InningNo == 1)
                {
                    GetCorrectOption(mQuestions, mFirstInnningQuestions);
                }
                //    else if (mQuestions.InningNo == 2)
                //    {
                //        GetCorrectOption(mQuestions, mSecondInnningQuestions);
                //    }
                //}

            }

            return mQuestionsList;
        }

        private static void GetCorrectOption(Questions vQuestion, PreMatchQuestions vPreMatchQuestions)
        {
            switch (vQuestion.QuestionCode)
            {
                case "SIX_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToHitMostSixes);
                    break;
                case "FOUR_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToHitMostFours);
                    break;
                case "WCKT_GVN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToConcedeMostWickets);
                    break;
                case "WCKT_TKN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToTakeMostWickets);
                    break;
                case "EXTRA_GVN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToConcedeMostExtras);
                    break;
                case "MAX_PP_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToScoreMaxRunsInPP);
                    break;

                case "WIN_TEAM":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TeamToWinMatch);
                    break;
                case "SIX_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.NoOfSixesInMatch);
                    break;
                case "FOUR_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.NoOfFoursInMatch);
                    break;
                case "WCKT_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.NoOfWicketsInMatch);
                    break;
                case "RUN_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.TotalRunsInMatch);
                    break;
                case "HIG_SCOR_MATCH":
                    ResolveQuestionOption(vQuestion, vPreMatchQuestions.HighestScoreInMatch);
                    break;
            }
        }

        private static void GetCorrectOption(Questions vQuestion, InningQuestions vInningQuestions)
        {

            switch (vQuestion.QuestionCode)
            {
                case "SIX_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToHitMostSixes);
                    break;
                case "FOUR_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToHitMostFours);
                    break;
                case "WCKT_TKN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToTakeMostWickets);
                    break;
                case "CATCH_TKN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToTakeMostCatchs);
                    break;
                case "EXTRA_GVN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToConcedeMostExtras);
                    break;
                case "RUN_GVN_PLYR":
                    ResolveQuestionOption(vQuestion, vInningQuestions.PlayerToConcedeMostRuns);
                    break;

                case "WCKT_TKN_ING":
                    ResolveQuestionOption(vQuestion, vInningQuestions.WicketsInInning);
                    break;
                case "WCKT_TKN_PP":
                    ResolveQuestionOption(vQuestion, vInningQuestions.WicketsInPP);
                    break;
                case "EXTRA_TKN_ING":
                    ResolveQuestionOption(vQuestion, vInningQuestions.ExtrasInInning);
                    break;
                case "RUN_PP":
                    ResolveQuestionOption(vQuestion, vInningQuestions.RunsInPP);
                    break;
                case "RUN_ING":
                    ResolveQuestionOption(vQuestion, vInningQuestions.RunsInInning);
                    break;
                case "RUN_L5":
                    ResolveQuestionOption(vQuestion, vInningQuestions.RunsInLst5Overs);
                    break;
            }
        }

        private static void ResolveQuestionOption(Questions vQuestions, List<string> vAnswers)
        {
            foreach (Option mOption in vQuestions.Options)
            {
                if (vAnswers.Count == 0 || vAnswers == null)
                {
                    if (mOption.AssetType.ToLower() == "none")
                    {
                        mOption.IsCorrect = 1;
                    }
                }
                else if (vAnswers.IndexOf(mOption.AssetId.ToString()) > -1)
                {
                    mOption.IsCorrect = 1;
                }
            }
        }

        private static void ResolveQuestionOption(Questions vQuestions, int vAnswer)
        {
            foreach (Option mOption in vQuestions.Options)
            {
                if (vQuestions.QuestionType.ToLower() == "tem")
                {
                    if (vAnswer == 0 && mOption.AssetType.ToLower() == "draw" || mOption.AssetType.ToLower() == "none")
                    {
                        mOption.IsCorrect = 1;
                    }
                    else if (mOption.AssetId == vAnswer)
                    {
                        mOption.IsCorrect = 1;
                    }
                }
                else if (mOption.MinVal <= vAnswer && (mOption.MaxVal >= vAnswer || mOption.MaxVal == null))
                {
                    mOption.IsCorrect = 1;
                }


                // if (vAnswer == 0 && (mOption.AssetType.ToLower() == "draw" || mOption.AssetType.ToLower() == "none"))
                //    {
                //        mOption.IsCorrect = 1;
                //        break;
                //    }
                //    else if (mOption.AssetId == vAnswer)
                //    {
                //        mOption.IsCorrect = 1;
                //    }           
                //else if (vAnswer != 0 && mOption.MinVal <= vAnswer && (mOption.MaxVal >= vAnswer || mOption.MaxVal == null))
                //{
                //    mOption.IsCorrect = 1;
                //}

            }
        }

    }
}
