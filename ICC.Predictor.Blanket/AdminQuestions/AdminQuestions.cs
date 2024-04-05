using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Enums;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICC.Predictor.Blanket.AdminQuestions
{
    public class AdminQuestions : BaseBlanket
    {
        private readonly DataAccess.AdminQuestions.AdminQuestions _QuestionContext;
        private readonly int _TourId;

        public AdminQuestions(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _QuestionContext = new DataAccess.AdminQuestions.AdminQuestions(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public int SaveQuestions(MatchQuestions model)
        {
            int matchId = model.MatchId;
            int questionId = model.QuestionId; string questionDesc = model.QuestionDesc.Trim();
            string questionType = model.QuestionType;
            int questionStatus = model.QuestionStatus;
            List<int> optionIds = new List<int>(); List<string> optionDescs = new List<string>();
            List<int> isCorrects = new List<int>();
            int retVal = -40;
            try
            {
                //model.QuestionDesc = model.QuestionDesc.Trim();
                int i = 1;
                foreach (var option in model.Options)
                {
                    if (option.OptionDesc != string.Empty)
                    {
                        if (option.OptionId == 0)
                        {
                            option.OptionId = i;
                        }
                        optionIds.Add(option.OptionId);
                        optionDescs.Add(option.OptionDesc.Trim());
                        isCorrects.Add(option.IsCorrectBool ? 1 : 0);
                        i++;
                    }
                }
                int optType = 1;
                retVal = _QuestionContext.SaveQuestions(optType, _TourId, matchId, questionId, questionDesc, questionType, questionStatus, optionIds.ToArray(), optionDescs.ToArray(), isCorrects.ToArray());

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.SaveQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return retVal;
        }

        public List<MatchQuestions> GetMatchQuestions(int matchId)
        {
            int retVal = -40;
            List<MatchQuestions> questions = new List<MatchQuestions>();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                int optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetMatchQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return questions;
        }

        public List<MatchQuestions> GetFilteredQuestions(int matchId, string questionStatus)
        {
            int retVal = -40;
            List<MatchQuestions> questions = new List<MatchQuestions>();
            List<MatchQuestions> filteredquestions = new List<MatchQuestions>();
            int questionsStatusInt = Convert.ToInt32(questionStatus);
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                int optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal).Where(a => a.QuestionOccurrence.ToLower() != "prm").ToList();
                if (questionsStatusInt == Convert.ToInt32(QuestionStatus.Published) || questionsStatusInt == Convert.ToInt32(QuestionStatus.Locked))
                {
                    filteredquestions = questions.Where(a => a.QuestionStatus == Convert.ToInt32(QuestionStatus.Published) || a.QuestionStatus == Convert.ToInt32(QuestionStatus.Locked)).ToList();
                }
                else
                {
                    filteredquestions = questions.Where(a => a.QuestionStatus == (questionsStatusInt == -2 ? a.QuestionStatus : questionsStatusInt)).ToList();
                }

                filteredquestions.Reverse();

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetFilteredQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return filteredquestions;
        }

        public MatchQuestions GetMatchQuestionsDetail(int matchId, int questionId)
        {
            int retVal = -40;
            MatchQuestions questions = new MatchQuestions();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                int optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal)
                    .Where(a => a.QuestionId == questionId).FirstOrDefault();

                foreach (var option in questions.Options)
                {
                    option.IsCorrectBool = option.IsCorrect == 1;
                }

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetMatchQuestionsDetail", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return questions;
        }

        public int AbandonMatch(int abandonMatchId)
        {

            int retVal = -40;
            try
            {
                //model.QuestionDesc = model.QuestionDesc.Trim();
                int optType = 1;
                retVal = _QuestionContext.AbandonMatch(optType, _TourId, abandonMatchId);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.SaveQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return retVal;
        }
    }
}
