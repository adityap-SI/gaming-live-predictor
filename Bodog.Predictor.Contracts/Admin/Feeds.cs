using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace Bodog.Predictor.Contracts.Admin
{
    [XmlRoot]
    public class calendar
    {
        [XmlElement]
        public List<match> match { get; set; }
    }

    public class match
    {
        [XmlAttribute]
        public string daynight { get; set; }

        [XmlAttribute]
        public string gmt_offset { get; set; }

        [XmlAttribute]
        public string league { get; set; }

        [XmlAttribute]
        public string live { get; set; }

        [XmlAttribute]
        public string livecoverage { get; set; }

        [XmlAttribute]
        public string match_Id { get; set; }

        [XmlAttribute]
        public string matchdate_gmt { get; set; }

        [XmlAttribute]
        public string end_matchdate_gmt { get; set; }

        [XmlAttribute]
        public string matchdate_ist { get; set; }

        [XmlAttribute]
        public string end_matchdate_ist { get; set; }

        [XmlAttribute]
        public string matchdate_local { get; set; }

        [XmlAttribute]
        public string end_matchdate_local { get; set; }

        [XmlAttribute]
        public string matchfile { get; set; }

        [XmlAttribute]
        public string matchnumber { get; set; }

        [XmlAttribute]
        public string matchresult { get; set; }

        [XmlAttribute]
        public string matchstatus { get; set; }

        [XmlAttribute]
        public string matchtime_gmt { get; set; }

        [XmlAttribute]
        public string end_matchtime_gmt { get; set; }

        [XmlAttribute]
        public string matchtime_ist { get; set; }

        [XmlAttribute]
        public string end_matchtime_ist { get; set; }

        [XmlAttribute]
        public string matchtime_local { get; set; }

        [XmlAttribute]
        public string end_matchtime_local { get; set; }

        [XmlAttribute]
        public string matchtype { get; set; }

        [XmlAttribute]
        public string priority { get; set; }

        [XmlAttribute]
        public string recent { get; set; }

        [XmlAttribute]
        public string series_end_date { get; set; }

        [XmlAttribute]
        public string series_Id { get; set; }

        [XmlAttribute]
        public string series_short_display_name { get; set; }

        [XmlAttribute]
        public string series_start_date { get; set; }

        [XmlAttribute]
        public string series_type { get; set; }

        [XmlAttribute]
        public string seriesname { get; set; }

        [XmlAttribute]
        public string stage { get; set; }

        [XmlAttribute]
        public string teama { get; set; }

        [XmlAttribute]
        public string teama_short { get; set; }

        [XmlAttribute]
        public string teama_Id { get; set; }

        [XmlAttribute]
        public string teamb { get; set; }

        [XmlAttribute]
        public string teamb_short { get; set; }

        [XmlAttribute]
        public string teamb_Id { get; set; }

        [XmlAttribute]
        public string toss_elected_to { get; set; }

        [XmlAttribute]
        public string toss_won_by { get; set; }

        [XmlAttribute]
        public string tour_Id { get; set; }

        [XmlAttribute]
        public string tourname { get; set; }

        [XmlAttribute]
        public string upcoming { get; set; }

        [XmlAttribute]
        public string venue { get; set; }

        [XmlAttribute]
        public string venue_Id { get; set; }

        [XmlAttribute]
        public string winningmargin { get; set; }

        [XmlAttribute]
        public string winningteam_Id { get; set; }

        [XmlAttribute]
        public string group { get; set; }
    }

    #region " Commentary "

    //[XmlRoot(ElementName = "Node")]
    //public class Node
    //{
    //    [XmlElement(ElementName = "Runs")]
    //    public string Runs { get; set; }

    //    [XmlElement(ElementName = "Zone")]
    //    public string Zone { get; set; }

    //    [XmlElement(ElementName = "Angle")]
    //    public string Angle { get; set; }

    //    [XmlElement(ElementName = "Length")]
    //    public string Length { get; set; }

    //    [XmlElement(ElementName = "BallLineLength")]
    //    public string BallLineLength { get; set; }

    //    [XmlElement(ElementName = "BallSpeed")]
    //    public string BallSpeed { get; set; }

    //    [XmlElement(ElementName = "BatDetails")]
    //    public string BatDetails { get; set; }

    //    [XmlElement(ElementName = "BowlDetails")]
    //    public string BowlDetails { get; set; }

    //    [XmlElement(ElementName = "ThisOver")]
    //    public string ThisOver { get; set; }

    //    [XmlElement(ElementName = "Animation")]
    //    public string Animation { get; set; }

    //    [XmlElement(ElementName = "Details")]
    //    public string Details { get; set; }

    //    [XmlElement(ElementName = "Commentary")]
    //    public string Commentary { get; set; }

    //    [XmlElement(ElementName = "NonStrBatDetails")]
    //    public string NonStrBatDetails { get; set; }

    //    [XmlAttribute(AttributeName = "Id")]
    //    public string Id { get; set; }

    //    [XmlAttribute(AttributeName = "Over")]
    //    public string Over { get; set; }

    //    [XmlAttribute(AttributeName = "Ball")]
    //    public string Ball { get; set; }

    //    [XmlAttribute(AttributeName = "Batsman")]
    //    public string Batsman { get; set; }

    //    [XmlAttribute(AttributeName = "Bowler")]
    //    public string Bowler { get; set; }

    //    [XmlAttribute(AttributeName = "Scored")]
    //    public string Scored { get; set; }

    //    [XmlAttribute(AttributeName = "Wicket")]
    //    public string Wicket { get; set; }

    //    [XmlAttribute(AttributeName = "Batsman_Id")]
    //    public string Batsman_Id { get; set; }

    //    [XmlAttribute(AttributeName = "Bowler_Id")]
    //    public string Bowler_Id { get; set; }

    //    [XmlAttribute(AttributeName = "MissedBall")]
    //    public string MissedBall { get; set; }

    //    [XmlAttribute(AttributeName = "NonStriker")]
    //    public string NonStriker { get; set; }

    //    [XmlAttribute(AttributeName = "NonStriker_Id")]
    //    public string NonStriker_Id { get; set; }

    //    [XmlAttribute(AttributeName = "TimeOfDay")]
    //    public string TimeOfDay { get; set; }

    //    [XmlElement(ElementName = "Score")]
    //    public string Score { get; set; }
    //}

    //[XmlRoot(ElementName = "Innings")]
    //public class Innings
    //{
    //    [XmlElement(ElementName = "Node")]
    //    public List<Node> Node { get; set; }

    //    [XmlAttribute(AttributeName = "Number")]
    //    public string Number { get; set; }

    //    [XmlAttribute(AttributeName = "Batting")]
    //    public string Batting { get; set; }

    //    [XmlAttribute(AttributeName = "Bowling")]
    //    public string Bowling { get; set; }

    //    [XmlAttribute(AttributeName = "Battingteam_Id")]
    //    public string Battingteam_Id { get; set; }

    //    [XmlAttribute(AttributeName = "Bowlingteam_Id")]
    //    public string Bowlingteam_Id { get; set; }

    //    [XmlText]
    //    public string Text { get; set; }
    //}

    //[XmlRoot(ElementName = "Match")]
    //public class Match
    //{
    //    [XmlElement(ElementName = "Preview")]
    //    public string Preview { get; set; }

    //    [XmlElement(ElementName = "Innings")]
    //    public List<Innings> Innings { get; set; }

    //    [XmlAttribute(AttributeName = "match_Id")]
    //    public string Match_Id { get; set; }
    //}

    #endregion " Commentary "

    #region " Squads  "

    [XmlRoot(ElementName = "coach")]
    public class Coach
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "team_personnel")]
    public class Team_personnel
    {
        [XmlElement(ElementName = "coach")]
        public Coach Coach { get; set; }
    }

    [XmlRoot(ElementName = "player")]
    public class Player
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "skill")]
        public string Skill { get; set; }

        [XmlAttribute(AttributeName = "battingstyle")]
        public string Battingstyle { get; set; }

        [XmlAttribute(AttributeName = "bowlingstyle")]
        public string Bowlingstyle { get; set; }

        [XmlAttribute(AttributeName = "iscap")]
        public string Iscap { get; set; }

        [XmlAttribute(AttributeName = "iswk")]
        public string Iswk { get; set; }

        [XmlAttribute(AttributeName = "isprobable")]
        public string Isprobable { get; set; }

        [XmlAttribute(AttributeName = "isselected")]
        public string Isselected { get; set; }

        [XmlAttribute(AttributeName = "dropped_reason")]
        public string Dropped_reason { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "players")]
    public class PlayersList
    {
        [XmlElement(ElementName = "player")]
        public List<Player> Player { get; set; }
    }

    [XmlRoot(ElementName = "squad_list")]
    public class Squad_list
    {
        [XmlElement(ElementName = "team_personnel")]
        public Team_personnel Team_personnel { get; set; }

        [XmlElement(ElementName = "players")]
        public PlayersList Players { get; set; }

        [XmlAttribute(AttributeName = "series_id")]
        public string Series_id { get; set; }

        [XmlAttribute(AttributeName = "team_id")]
        public string Team_id { get; set; }

        [XmlAttribute(AttributeName = "league")]
        public string League { get; set; }
    }

    #endregion " Squads  "

    #region " Custom Contracts For Data Population "

    public class SITournament
    {
        public int tour_id { get; set; }
        public string tour_name { get; set; }
        public DateTime tour_start_date { get; set; }
        public DateTime tour_end_date { get; set; }
    }

    public class SISeries
    {
        public int tour_id { get; set; }
        public int series_Id { get; set; }
        public string series_type { get; set; }
        public string seriesname { get; set; }
        public string series_short_display_name { get; set; }
        public DateTime series_start_date { get; set; }
        public DateTime series_end_date { get; set; }
        public string comp_type { get; set; }
    }

    public class SITeam
    {
        public int tour_id { get; set; }
        public int series_id { get; set; }
        public int team_id { get; set; }
        public string team_name { get; set; }
        public string team_short { get; set; }
    }

    public class SIPlayer
    {
        public int tour_id { get; set; }
        public int team_id { get; set; }
        public int series_id { get; set; }
        public int playerid { get; set; }
        public string player_name { get; set; }
        public string player_display_name { get; set; }
        public string skill_name { get; set; }
        public int skill_id { get; set; }
    }

    public class SIMatch
    {
        public int tour_id { get; set; }
        public int series_id { get; set; }
        public int match_Id { get; set; }

        public string venue { get; set; }

        public int home_team_id { get; set; }
        public string home_team_name { get; set; }
        public string home_team_short_name { get; set; }

        public int away_team_id { get; set; }
        public string away_team_name { get; set; }
        public string away_team_short_name { get; set; }

        public DateTime matchdate_ist { get; set; }
        public string match_name { get; set; }

        public string match_file { get; set; }
        public string match_number { get; set; }
        public string match_result { get; set; }
        public string match_status { get; set; }
        public string match_time_gmt { get; set; }
        public string match_time_ist { get; set; }
        public string match_time_local { get; set; }
        public string match_type { get; set; }
    }

    #endregion " Custom Contracts For Data Population "

    #region " Helper "

    public class JsonResult
    {
        public bool error { get; set; }
        public string error_message { get; set; }
        public string responseString { get; set; }
        public DataSet dataSet { get; set; }
    }

    public class SeriesFormat
    {
        private readonly List<string> skillsList = new List<string>()
        {
            "Test",
            "ODI",
            "T20"
        };

        public SeriesFormat(string value)
        {
            Value = skillsList.FindIndex(a => a.ToLower() == value);
        }

        public Int32 Value { get; private set; }
    }

    #endregion " Helper "

    #region " Match Feed "

    public class Matchdetail
    {
        public string Team_Home { get; set; }
        public string Team_Away { get; set; }

        public string Tosswonby { get; set; }
        public string Status { get; set; }
        public string Status_Id { get; set; }
        public string Player_Match { get; set; }

        public string Winningteam { get; set; }
        public string Winmargin { get; set; }

        public MatchInfo Match { get; set; }
    }

    public class MatchInfo
    {
        public String Id { get; set; }
        public String Code { get; set; }
    }
    public class BatsmanStats
    {
        public string Batsman { get; set; }
        public string Runs { get; set; }
        public string Balls { get; set; }
        public string Fours { get; set; }
        public string Sixes { get; set; }
        public string Dots { get; set; }
        public string Strikerate { get; set; }
        public string Dismissal { get; set; }
        public string Howout { get; set; }
        public string Bowler { get; set; }
        public string Fielder { get; set; }
        public bool? Isonstrike { get; set; }
    }

    public class BowlerStats
    {
        public string Bowler { get; set; }
        public string Overs { get; set; }
        public string Maidens { get; set; }
        public string Runs { get; set; }
        public string Wickets { get; set; }
        public string Economyrate { get; set; }
        public string Noballs { get; set; }
        public string Wides { get; set; }
        public string Dots { get; set; }
        public bool? Isbowlingtandem { get; set; }
        public bool? Isbowlingnow { get; set; }
    }

    public class Innings
    {
        public string Number { get; set; }
        public string Battingteam { get; set; }
        public string Total { get; set; }
        public string Wickets { get; set; }
        public string Overs { get; set; }
        public string Runrate { get; set; }
        public string Byes { get; set; }
        public string Legbyes { get; set; }
        public string Wides { get; set; }
        public string Noballs { get; set; }
        public string Penalty { get; set; }
        public string AllottedOvers { get; set; }
        public List<BatsmanStats> Batsmen { get; set; }
        public List<BowlerStats> Bowlers { get; set; }
        public string Target { get; set; }
    }

    public class PlayerInfo
    {
        public string Position { get; set; }
        public string Name_Full { get; set; }
        public bool Iscaptain { get; set; }
    }

    public class TeamInfo
    {
        public string Name_Full { get; set; }
        public string Name_Short { get; set; }
        public Dictionary<String, PlayerInfo> Players { get; set; }
    }

    public class Teams
    {
        public Dictionary<String, TeamInfo> TeamsList { get; set; }
    }

    public class MatchFeed
    {
        public Matchdetail Matchdetail { get; set; }
        public List<Innings> Innings { get; set; }
        //public Teams Teams { get; set; }
        public Dictionary<String, TeamInfo> Teams { get; set; }
    }

    #endregion


    #region " Scoring "

    #region " Player Stats "

    public class MatchPlayerStats
    {
        public String MatchId { get; set; }
        public Int64 HomeTeamId { get; set; }
        public Int64 AwayTeamId { get; set; }

        public Int64 TossWonById { get; set; }
        public String Status { get; set; }
        public Int64 WinningTeamId { get; set; }
        public String CurrentInning { get; set; }

        public List<PlayerStats> PlayerStats { get; set; }
    }

    public class PlayerStats
    {
        public Int64 PlayerId { get; set; }
        public String PlayerName { get; set; }
        public Int64 TeamId { get; set; }

        #region " Batting Stats "

        public Int32 RunsScored { get; set; }
        public Int32 SixesHit { get; set; }
        public Int32 FoursHit { get; set; }
        public Int32 BoundriesHit
        {
            get
            {
                return FoursHit + SixesHit;
            }
        }

        #endregion

        #region " Bowling Stats "

        public Int32 Wickets { get; set; }
        public Int32 RunsGiven { get; set; }
        public Int32 WideBalls { get; set; }
        public Int32 NoBalls { get; set; }
        public Int32 ExtrasNoWide
        {
            get
            {
                return WideBalls + NoBalls;
            }
        }

        #endregion

        #region " Fielding Stats "

        public Int32 Catches { get; set; }

        #endregion

    }

    #endregion

    public class Lineups
    {
        public String TeamId { get; set; }
        public String PlayerId { get; set; }
        public String PlayerName { get; set; }
    }

    #endregion

    #region " Questions "

    public class Option
    {
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public string OptionDec { get; set; }
        public int AssetId { get; set; }
        public string AssetType { get; set; }
        public int IsCorrect { get; set; }
        public int MinVal { get; set; }
        public int? MaxVal { get; set; }
    }

    public class Questions
    {
        public int QuestionId { get; set; }
        public int QuestionNo { get; set; }
        public int MatchId { get; set; }
        public int InningNo { get; set; }
        public string QuestionDec { get; set; }
        public int QuestionStatus { get; set; }
        public string QuestionType { get; set; }
        public string QuestionOccurrence { get; set; }
        public object OptionJson { get; set; }
        public object OptionLists { get; set; }
        public string QuestionCode { get; set; }
        public List<Option> Options { get; set; }
    }


    #endregion

    #region " SI- Fixtures "

    public class Match
    {
        public string daynight { get; set; }
        public string gmt_offset { get; set; }
        public string group { get; set; }
        public string league { get; set; }
        public string live { get; set; }
        public string livecoverage { get; set; }
        public string match_Id { get; set; }
        public string matchfile { get; set; }
        public string matchnumber { get; set; }
        public string matchresult { get; set; }
        public string matchstatus { get; set; }
        public string matchdate_gmt { get; set; }
        public string matchdate_ist { get; set; }
        public string matchdate_local { get; set; }
        public string matchtime_gmt { get; set; }
        public string matchtime_ist { get; set; }
        public string matchtime_local { get; set; }
        public string end_matchdate_gmt { get; set; }
        public string end_matchdate_ist { get; set; }
        public string end_matchdate_local { get; set; }
        public string end_matchtime_gmt { get; set; }
        public string end_matchtime_ist { get; set; }
        public string end_matchtime_local { get; set; }
        public string matchtype { get; set; }
        public string priority { get; set; }
        public string recent { get; set; }
        public string series_Id { get; set; }
        public string seriesname { get; set; }
        public string series_short_display_name { get; set; }
        public string series_type { get; set; }
        public string series_start_date { get; set; }
        public string series_end_date { get; set; }
        public string toss_elected_to { get; set; }
        public string toss_won_by { get; set; }
        public string stage { get; set; }
        public string teama { get; set; }
        public string teama_short { get; set; }
        public string teama_Id { get; set; }
        public string teamb { get; set; }
        public string teamb_short { get; set; }
        public string teamb_Id { get; set; }
        public string tour_Id { get; set; }
        public string tourname { get; set; }
        public string upcoming { get; set; }
        public string venue { get; set; }
        public string venue_Id { get; set; }
        public string winningmargin { get; set; }
        public string winningteam_Id { get; set; }
        public string current_score { get; set; }
        public string current_batting_team { get; set; }
        public string teamscores { get; set; }
        public string inn_team_1 { get; set; }
        public string inn_score_1 { get; set; }
        public string inn_team_2 { get; set; }
        public string inn_score_2 { get; set; }
        public string inn_team_3 { get; set; }
        public string inn_score_3 { get; set; }
        public string inn_team_4 { get; set; }
        public string inn_score_4 { get; set; }
    }

    public class MatchesList
    {
        public List<Match> matches { get; set; }
    }

    public class AllFixtures
    {
        public MatchesList data { get; set; }
    }

    #endregion

    #region " Admin LeaderBoards"

    public class Reports
    {
        public List<AdminLeaderBoard> LeaderBoardList { get; set; }
    }

    public class AdminLeaderBoard
    {
        public String UserId { get; set; }
        public String UserTeamId { get; set; }
        public String GUID { get; set; }
        public String TeamName { get; set; }
        public String FullName { get; set; }
        public String RankNo { set; get; }
        public String Rank { set; get; }
        public String Trend { set; get; }
        public Int64 TotalMember { get; set; }
        public string Notation { set; get; }
        public String CurrentGamedayPoints { set; get; }
        public String Points { set; get; }
        public String PhoneNumber { set; get; }
        public String EmailId { set; get; }
        public String SocialId { set; get; }
    }
    #endregion " Admin LeaderBoards "

    
}