using System;
using System.Collections.Generic;

namespace Bodog.Predictor.Contracts.Feeds
{
    #region " Fixtures "

    public class Fixtures
    {
        public Int32 MatchId { get; set; }
        public Int32 TeamGamedayId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public Int32 phaseId { get; set; }
        public Int32 GamedayId { get; set; }
        public String Date { get; set; }
        public String Deadlinedate { get; set; }

        public Int32 TeamAId { get; set; }
        public String TeamAName { get; set; }
        public String TeamAShortName { get; set; }
        public String TeamACountryCode { get; set; }
        public decimal TeamAScore { get; set; }

        public Int32 TeamBId { get; set; }
        public String TeamBName { get; set; }
        public String TeamBShortName { get; set; }
        public String TeamBCountryCode { get; set; }
        public decimal TeamBScore { get; set; }

        public String MatchdayName { get; set; }
        public Int32 MatchStatus { get; set; }
        public Int32 Is_Lineup_Process { get; set; }
        public Int32 Is_Toss_Process { get; set; }
        public Int32 Inning_1_BWL_Teamid { get; set; }
        public Int32 Inning_1_BAT_Teamid { get; set; }
        public Int32 Inning_2_BAT_Teamid { get; set; }
        public Int32 Inning_2_BWL_Teamid { get; set; }
        public Int32 Match_Inning_Status { get; set; }
        public String Matchfile { get; set; }
        public String Venue { get; set; }
        public Int32 IsQuestionAnswerProcess { get; set; }
    }


    #endregion " Fixtures "

    #region " Skills "

    public class Skills
    {
        public Int32 SkillId { get; set; }
        public String SkillName { get; set; }
    }

    #endregion

    #region " Match Questions "

    public class MatchQuestions
    {
        public Int32 QuestionId { get; set; }
        public Int32 QuestionNo { get; set; }
        public Int32 MatchId { get; set; }
        public Int32 InningNo { get; set; }
        public String QuestionDesc { get; set; }
        public Int32 QuestionStatus { get; set; }
        public String QuestionType { get; set; }
        public String QuestionCode { get; set; }
        public String QuestionOccurrence { get; set; }
        public String OptionJson { get; set; }
        public List<OptionList> OptionLists { get; set; }
        public List<Options> Options { get; set; }
        //public String LockedDate { get; set; }
        public String PublishedDate { get; set; }
        public String QuestionTime { get; set; }
        public String QuestionPoints { get; set; }
    }

    public class OptionList
    {
        public Int32 cf_questionid { get; set; }
        public Int32 cf_optionid { get; set; }
        public String option_dec { get; set; }
        public Int32 cf_assetid { get; set; }
        public String asset_type { get; set; }
        public Int32? is_correct { get; set; }
        public Int32? min_val { get; set; }
        public Int32? max_val { get; set; }
    }

    public class Options
    {
        public Int32 QuestionId { get; set; }
        public Int32 OptionId { get; set; }
        public String OptionDesc { get; set; }
        public Int32 AssetId { get; set; }
        public String AssetType { get; set; }
        public Int32 IsCorrect { get; set; }
        public Int32? MinVal { get; set; }
        public Int32? MaxVal { get; set; }
        public bool IsCorrectBool { get; set; }
    }

    public class UserPredictionSubmit
    {
        public Int32 MatchId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public Int32 QuestionId { get; set; }
        public Int32 OptionId { get; set; }
    }

    public class UserPredictionResult
    {
        //public Int32 QuestionId { get; set; }
        //public Int32 Question_No { get; set; }
        //public Int32 OptionId { get; set; }
        //public Int64 Points { get; set; }
        //public Int64 Rank { get; set; }
        public List<QuestionDetails> QuestionDetails { get; set; }
        public List<PointsRank> PointRank { get; set; }
    }

    public class QuestionDetails
    {
        public Int32 QuestionId { get; set; }
        public Int32 Question_No { get; set; }
        public Int32 OptionId { get; set; }
    }

    public class PointsRank
    {
        public Int32 Points { get; set; }
        public String Rank { get; set; }
    }

    public class GameDays
    {
        public Int32 TourGamedayId { get; set; }
    }

    #endregion " Match Questions "

    #region " Recent Results "
    public class RecentResults
    {
        public Int32 TeamID { get; set; }
        public Int64 TotalPlayed { get; set; }
        public Int64 TotalWin { get; set; }
        public Int64 TotalLoss { get; set; }
    }
    #endregion " Recent Results "

    #region " Inning Status "
    public class InningStatus
    {
        public Int32 MatchId { get; set; }
        public Int32 MatchStatus { get; set; }
        public Int32 MatchInningStatus { get; set; }
    }
    #endregion " Inning Status "

    #region " Played Gamedays "
    public class PlayedGamedays
    {
        public Int32 GamedayId { get; set; }
        //public String GamedayName { get; set; }
        public String GamedayName { get; set; }
        public String Matchdate { get; set; }
    }
    public class PlayedPhase
    {
        public Int32 PhaseId { get; set; }
        public String PhaseName { get; set; }
    }
    public class UserPlayedLB
    {
        // public String OverAll { get; set; }
        public List<PlayedGamedays> PlayedGamedays { get; set; }
        public List<PlayedPhase> PlayedPhase { get; set; }
    }
    #endregion

    #region " User Profile "
    public class UserDataPointsRank
    {
        public String EmailId { get; set; }
        public String PhoneNumber { get; set; }
        public String UserName { get; set; }
        public String ProfilePicture { get; set; }
        //public Int32 Points { get; set; }
        //public Int32 Rank { get; set; }
        public Int32 Points { get; set; }
        public String Rank { get; set; }
    }
    public class UserMatchData
    {
        public Int32 MatchId { get; set; }
        public String Date { get; set; }
        public Int32 Points { get; set; }
        public String Rank { get; set; }
        public String HomeTeamId { get; set; }
        public String HomeTeamName { get; set; }
        public String HomeTeamShortName { get; set; }
        public String AwayTeamId { get; set; }
        public String AwayTeamName { get; set; }
        public String AwayTeamShortName { get; set; }
    }

    public class UserProfile
    {
        public List<UserDataPointsRank> UserDataPointsRankList { get; set; }
        public List<UserMatchData> UserMatchDataList { get; set; }
    }

    #endregion " User Profile "

    #region " Currentgameday Matches "
    public class CurrentGamedayMatches
    {
        public Int32 MatchId { get; set; }
        public Int32 TeamGamedayId { get; set; }
        public Int32 TourGamedayId { get; set; }
        public String Date { get; set; }
        public Int32 MatchStatus { get; set; }
        public Int32 Live { get; set; }
        public Int32 Match_Inning_Status { get; set; }
    }
    #endregion

}