using System;
using System.Collections.Generic;

namespace ICC.Predictor.Contracts.Feeds
{
    #region " Fixtures "

    public class Fixtures
    {
        public int MatchId { get; set; }
        public int TeamGamedayId { get; set; }
        public int TourGamedayId { get; set; }
        public int phaseId { get; set; }
        public int GamedayId { get; set; }
        public string Date { get; set; }
        public string Deadlinedate { get; set; }

        public int TeamAId { get; set; }
        public string TeamAName { get; set; }
        public string TeamAShortName { get; set; }
        public string TeamACountryCode { get; set; }
        public decimal TeamAScore { get; set; }

        public int TeamBId { get; set; }
        public string TeamBName { get; set; }
        public string TeamBShortName { get; set; }
        public string TeamBCountryCode { get; set; }
        public decimal TeamBScore { get; set; }

        public string MatchdayName { get; set; }
        public int MatchStatus { get; set; }
        public int Is_Lineup_Process { get; set; }
        public int Is_Toss_Process { get; set; }
        public int Inning_1_BWL_Teamid { get; set; }
        public int Inning_1_BAT_Teamid { get; set; }
        public int Inning_2_BAT_Teamid { get; set; }
        public int Inning_2_BWL_Teamid { get; set; }
        public int Match_Inning_Status { get; set; }
        public string Matchfile { get; set; }
        public string Venue { get; set; }
        public int IsQuestionAnswerProcess { get; set; }
    }


    #endregion " Fixtures "

    #region " Skills "

    public class Skills
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
    }

    #endregion

    #region " Match Questions "

    public class MatchQuestions
    {
        public int QuestionId { get; set; }
        public int QuestionNo { get; set; }
        public int MatchId { get; set; }
        public int InningNo { get; set; }
        public string QuestionDesc { get; set; }
        public int QuestionStatus { get; set; }
        public string QuestionType { get; set; }
        public string QuestionCode { get; set; }
        public string QuestionOccurrence { get; set; }
        public string OptionJson { get; set; }
        public List<OptionList> OptionLists { get; set; }
        public List<Options> Options { get; set; }
        //public String LockedDate { get; set; }
        public string PublishedDate { get; set; }
        public string QuestionTime { get; set; }
        public string QuestionPoints { get; set; }
    }

    public class OptionList
    {
        public int cf_questionid { get; set; }
        public int cf_optionid { get; set; }
        public string option_dec { get; set; }
        public int cf_assetid { get; set; }
        public string asset_type { get; set; }
        public int? is_correct { get; set; }
        public int? min_val { get; set; }
        public int? max_val { get; set; }
    }

    public class Options
    {
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public string OptionDesc { get; set; }
        public int AssetId { get; set; }
        public string AssetType { get; set; }
        public int IsCorrect { get; set; }
        public int? MinVal { get; set; }
        public int? MaxVal { get; set; }
        public bool IsCorrectBool { get; set; }
    }

    public class UserPredictionSubmit
    {
        public int MatchId { get; set; }
        public int TourGamedayId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
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
        public int QuestionId { get; set; }
        public int Question_No { get; set; }
        public int OptionId { get; set; }
    }

    public class PointsRank
    {
        public int Points { get; set; }
        public string Rank { get; set; }
    }

    public class GameDays
    {
        public int TourGamedayId { get; set; }
    }

    #endregion " Match Questions "

    #region " Recent Results "
    public class RecentResults
    {
        public int TeamID { get; set; }
        public long TotalPlayed { get; set; }
        public long TotalWin { get; set; }
        public long TotalLoss { get; set; }
    }
    #endregion " Recent Results "

    #region " Inning Status "
    public class InningStatus
    {
        public int MatchId { get; set; }
        public int MatchStatus { get; set; }
        public int MatchInningStatus { get; set; }
    }
    #endregion " Inning Status "

    #region " Played Gamedays "
    public class PlayedGamedays
    {
        public int GamedayId { get; set; }
        //public String GamedayName { get; set; }
        public string GamedayName { get; set; }
        public string Matchdate { get; set; }
    }
    public class PlayedPhase
    {
        public int PhaseId { get; set; }
        public string PhaseName { get; set; }
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
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
        //public Int32 Points { get; set; }
        //public Int32 Rank { get; set; }
        public int Points { get; set; }
        public string Rank { get; set; }
    }
    public class UserMatchData
    {
        public int MatchId { get; set; }
        public string Date { get; set; }
        public int Points { get; set; }
        public string Rank { get; set; }
        public string HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamShortName { get; set; }
        public string AwayTeamId { get; set; }
        public string AwayTeamName { get; set; }
        public string AwayTeamShortName { get; set; }
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
        public int MatchId { get; set; }
        public int TeamGamedayId { get; set; }
        public int TourGamedayId { get; set; }
        public string Date { get; set; }
        public int MatchStatus { get; set; }
        public int Live { get; set; }
        public int Match_Inning_Status { get; set; }
    }
    #endregion

}