using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Enums;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bodog.Predictor.Blanket.AdminQuestions
{
    public class AdminQuestions : Common.BaseBlanket
    {
        private readonly DataAccess.AdminQuestions.AdminQuestions _QuestionContext;
        private readonly Int32 _TourId;

        public AdminQuestions(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _QuestionContext = new DataAccess.AdminQuestions.AdminQuestions(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public Int32 SaveQuestions(MatchQuestions model)
        {
            Int32 matchId = model.MatchId;
            Int32 questionId = model.QuestionId; String questionDesc = model.QuestionDesc.Trim();
            String questionType = model.QuestionType;
            Int32 questionStatus = model.QuestionStatus;
            List<int> optionIds = new List<int>(); List<String> optionDescs = new List<string>();
            List<int> isCorrects = new List<int>();
            Int32 retVal = -40;
            try
            {
                //model.QuestionDesc = model.QuestionDesc.Trim();
                Int32 i = 1;
                foreach (var option in model.Options)
                {
                    if (option.OptionDesc != String.Empty)
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
                Int32 optType = 1;
                retVal = _QuestionContext.SaveQuestions(optType, _TourId, matchId, questionId, questionDesc, questionType, questionStatus, optionIds.ToArray(), optionDescs.ToArray(), isCorrects.ToArray());

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.SaveQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return retVal;
        }

        public List<MatchQuestions> GetMatchQuestions(Int32 matchId)
        {
            Int32 retVal = -40;
            List<MatchQuestions> questions = new List<MatchQuestions>();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                Int32 optType = 1;
                questions = _QuestionContext.GetMatchQuestions(optType, _TourId, matchId, ref httpMeta, ref retVal);

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Questions.Questions.GetMatchQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return questions;
        }

        public List<MatchQuestions> GetFilteredQuestions(Int32 matchId, String questionStatus)
        {
            Int32 retVal = -40;
            List<MatchQuestions> questions = new List<MatchQuestions>();
            List<MatchQuestions> filteredquestions = new List<MatchQuestions>();
            Int32 questionsStatusInt = Convert.ToInt32(questionStatus);
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                Int32 optType = 1;
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

        public MatchQuestions GetMatchQuestionsDetail(Int32 matchId, Int32 questionId)
        {
            Int32 retVal = -40;
            MatchQuestions questions = new MatchQuestions();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                Int32 optType = 1;
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

        public Int32 AbandonMatch(Int32 abandonMatchId)
        {

            Int32 retVal = -40;
            try
            {
                //model.QuestionDesc = model.QuestionDesc.Trim();
                Int32 optType = 1;
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
