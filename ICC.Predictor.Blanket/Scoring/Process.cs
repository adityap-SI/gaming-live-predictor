using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICC.Predictor.Blanket.Scoring
{
    public class Process : Common.BaseBlanket
    {
        private readonly Answers _Answers;
        private readonly DataAccess.Scoring.Answers _AnswersDB;
        private readonly BackgroundServices.GameLocking _GameLocking;
        private readonly int _TourId;

        public Process(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _Answers = new Answers(appSettings, aws, postgre, redis, cookies, asset);
            _AnswersDB = new DataAccess.Scoring.Answers(postgre);
            _GameLocking = new BackgroundServices.GameLocking(_AppSettings, null, _AWS, _Postgre, _Redis, _Cookies, _Asset);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public bool CalculateAnswers(Fixtures vFixture)
        {
            bool success = false;
            int OptType = 1;

            List<Questions> mQuestionsList = new List<Questions>();
            mQuestionsList = _Answers.GetQuestionsWithAnswers(vFixture.Matchfile, vFixture.MatchId)
                            .Where(x => x.QuestionOccurrence.ToLower() == "prm").ToList();

            if (mQuestionsList != null)
            {
                foreach (Questions mQuestion in mQuestionsList)
                {
                    List<Option> mCorrectOptions = mQuestion.Options.Where(c => c.IsCorrect == 1).ToList();
                    if (mCorrectOptions == null || mCorrectOptions.Count <= 0)
                    {
                        mCorrectOptions.Add(new Option
                        {
                            OptionId = 0
                        });
                    }
                    if (mCorrectOptions != null)
                    {
                        long retVal = -50;
                        retVal = _AnswersDB.SubmitQuestionAnswer(OptType, _TourId, vFixture.MatchId, mQuestion.QuestionId, mCorrectOptions);
                        if (retVal == 1)
                            success = true;
                        else
                            break;
                    }
                }
            }

            return success;
        }

        public bool QuestionAnswerProcessUpdate(int matchId)
        {
            bool success = false;
            int OptType = 1;
            long retVal = -50;

            retVal = _AnswersDB.QuestionAnswerProcessUpdate(OptType, _TourId, matchId);

            if (retVal == 1)
                success = true;

            return success;
        }

        public bool SubmitMatchWinTeam(Fixtures fixture)
        {
            bool success = false;
            int OptType = 1;
            long retVal = -50;
            int matchId = fixture.MatchId;
            int teamId = 0;

            MatchFeed mMatchFeed = new MatchFeed();
            List<Match> mMatches = new List<Match>();
            Match mMatch = new Match();

            mMatchFeed = _GameLocking.GetMatchScoresFeed(fixture.Matchfile);



            if (string.IsNullOrEmpty(mMatchFeed.Matchdetail.Winningteam) && mMatchFeed.Matchdetail.Winningteam.SmartIntParse() == 0)
            {//return false;
                mMatches = _GameLocking.GetFixturesFeed();
                mMatch = mMatches.Where(c => c.match_Id == fixture.MatchId.ToString()).FirstOrDefault();
                teamId = mMatch.winningteam_Id.SmartIntParse();
            }
            else
                teamId = mMatchFeed.Matchdetail.Winningteam.SmartIntParse();

            retVal = _AnswersDB.SubmitMatchWinTeam(OptType, _TourId, matchId, teamId);

            if (retVal == 1)
                success = true;

            return success;
        }

        public bool CalculateFirstInningAnswers(Fixtures vFixture)
        {
            bool success = false;
            int OptType = 1;

            List<Questions> mQuestionsList = new List<Questions>();
            mQuestionsList = _Answers.GetFirstInningQuestionsWithAnswers(vFixture.Matchfile, vFixture.MatchId);

            if (mQuestionsList != null)
            {
                foreach (Questions mQuestion in mQuestionsList)
                {
                    List<Option> mCorrectOptions = mQuestion.Options.Where(c => c.IsCorrect == 1).ToList();
                    if (mCorrectOptions == null || mCorrectOptions.Count <= 0)
                    {
                        mCorrectOptions.Add(new Option
                        {
                            OptionId = 0
                        });
                    }
                    if (mCorrectOptions != null)
                    {
                        long retVal = -50;
                        retVal = _AnswersDB.SubmitQuestionAnswer(OptType, _TourId, vFixture.MatchId, mQuestion.QuestionId, mCorrectOptions);
                        if (retVal == 1)
                            success = true;
                        else
                            break;
                    }
                }
            }

            return success;
        }
    }
}
