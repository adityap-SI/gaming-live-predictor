using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Bodog.Predictor.DataInitializer.AdminQuestions
{
    public class AdminQuestions
    {
        public static List<MatchQuestions> InitializeQuestionId(NpgsqlCommand mNpgsqlCmd, List<String> cursors, ref int retVal)
        {

            DataSet ds = null;
            List<MatchQuestions> QuestionId = new List<MatchQuestions>();
            retVal = -60;

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                MatchQuestions question = new MatchQuestions();

                                question.QuestionId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_questionid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_questionid"].ToString());
                                question.QuestionNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_no"].ToString());
                                question.MatchId = Convert.IsDBNull(ds.Tables[0].Rows[i]["cf_matchid"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["cf_matchid"].ToString());
                                question.InningNo = Convert.IsDBNull(ds.Tables[0].Rows[i]["inning_no"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["inning_no"].ToString());
                                question.QuestionDesc = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_dec"]) ? "" : ds.Tables[0].Rows[i]["question_dec"].ToString();
                                question.QuestionStatus = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_status"]) ? 0 : Int32.Parse(ds.Tables[0].Rows[i]["question_status"].ToString());
                                question.QuestionType = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_type"]) ? "" : ds.Tables[0].Rows[i]["question_type"].ToString();
                                question.QuestionCode = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_sub_type"]) ? "" : ds.Tables[0].Rows[i]["question_sub_type"].ToString();
                                question.QuestionOccurrence = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_occurrence"]) ? "" : ds.Tables[0].Rows[i]["question_occurrence"].ToString();
                                //question.OptionJson = Convert.IsDBNull(ds.Tables[0].Rows[i]["option_json"]) ? "" : ds.Tables[0].Rows[i]["option_json"].ToString();
                                List<OptionList> OptionLists = Convert.IsDBNull(ds.Tables[0].Rows[i]["option_json"]) ? new List<OptionList>() : GenericFunctions.Deserialize<List<OptionList>>(ds.Tables[0].Rows[i]["option_json"].ToString());
                                question.PublishedDate = Convert.IsDBNull(ds.Tables[0].Rows[i]["locked_date"]) ? "" : ds.Tables[0].Rows[i]["locked_date"].ToString();
                                question.QuestionTime = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_time"]) ? "0" : Math.Truncate(Convert.ToDouble(ds.Tables[0].Rows[i]["question_time"].ToString())).ToString();
                                question.QuestionPoints = Convert.IsDBNull(ds.Tables[0].Rows[i]["question_points"]) ? "0" : ds.Tables[0].Rows[i]["question_points"].ToString();

                                question.Options = OptionLists.Select(s => new Options
                                {
                                    QuestionId = s.cf_questionid,
                                    OptionId = s.cf_optionid,
                                    OptionDesc = s.option_dec,
                                    AssetId = s.cf_assetid,
                                    AssetType = s.asset_type,
                                    IsCorrect = s.is_correct == null ? 0 : Convert.ToInt32(s.is_correct),
                                    MinVal = s.min_val,
                                    MaxVal = s.max_val
                                }).ToList<Options>();

                                QuestionId.Add(question);
                            }
                            retVal = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return QuestionId;
        }
    }
}
