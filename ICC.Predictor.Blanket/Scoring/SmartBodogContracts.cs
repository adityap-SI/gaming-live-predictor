using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ICC.Predictor.Blanket.Scoring
{
    public class PreMatchQuestions
    {
        private readonly MatchFeed _MatchFeed;
        private readonly MatchPlayerStats _MatchPlayerStats;
        private readonly XDocument _MatchAnalyticsDoc;
        private readonly Match _Match;

        public PreMatchQuestions(MatchFeed vMatchFeed, MatchPlayerStats vMatchPlayerStats, XDocument vMatchAnalyticsDoc, Match vMatch)
        {
            _MatchFeed = vMatchFeed;
            _MatchPlayerStats = vMatchPlayerStats;
            _MatchAnalyticsDoc = vMatchAnalyticsDoc;
            _Match = vMatch;
        }

        public List<string> TeamToHitMostSixes //SIX_TEAM
        {
            get
            {
                var SixesHit = _MatchPlayerStats.PlayerStats.GroupBy(o => o.TeamId).Select(s => new
                {
                    s.First().TeamId,
                    SixesHit = s.Sum(_ => _.SixesHit).ToString().SmartIntParse()
                }).ToList();

                return SixesHit.Where(s => s.SixesHit != 0 && s.SixesHit == SixesHit.Max(o => o.SixesHit))
                        .Select(t => t.TeamId.ToString()).ToList();
            }
        }
        public List<string> TeamToHitMostFours//FOUR_TEAM
        {
            get
            {
                var FoursHit = _MatchPlayerStats.PlayerStats.GroupBy(o => o.TeamId).Select(s => new
                {
                    s.First().TeamId,
                    FoursHit = s.Sum(_ => _.FoursHit).ToString().SmartIntParse()
                }).ToList();

                return FoursHit.Where(s => s.FoursHit != 0 && s.FoursHit == FoursHit.Max(o => o.FoursHit))
                        .Select(t => t.TeamId.ToString()).ToList();
            }
        }
        public List<string> TeamToConcedeMostWickets //WCKT_GVN_TEAM
        {
            get
            {
                var WicketsCon = _MatchFeed.Innings.Select(o => new { TeamId = o.Battingteam, Wickets = o.Wickets.SmartIntParse() });

                return WicketsCon.Where(s => s.Wickets != 0 && s.Wickets == WicketsCon.Max(o => o.Wickets))
                        .Select(t => t.TeamId.ToString()).ToList();
            }
        }
        public List<string> TeamToTakeMostWickets //WCKT_TKN_TEAM
        {
            get
            {
                var WicketsTaken = _MatchFeed.Innings.Select(o => new
                {
                    BowlingTeamId = _MatchFeed.Matchdetail.Team_Home == o.Battingteam ? _MatchFeed.Matchdetail.Team_Away : _MatchFeed.Matchdetail.Team_Home,
                    Wickets = o.Wickets.SmartIntParse()
                });

                return WicketsTaken.Where(s => s.Wickets != 0 && s.Wickets == WicketsTaken.Max(o => o.Wickets))
                        .Select(t => t.BowlingTeamId.ToString()).ToList();
            }
        }
        public List<string> TeamToConcedeMostExtras //EXTRA_GVN_TEAM
        {
            get
            {
                var ExtrasGiven = _MatchFeed.Innings.Select(o => new
                {
                    BowlingTeamId = _MatchFeed.Matchdetail.Team_Home == o.Battingteam ? _MatchFeed.Matchdetail.Team_Away : _MatchFeed.Matchdetail.Team_Home,
                    Extras = o.Byes.SmartIntParse() + o.Legbyes.SmartIntParse() + o.Wides.SmartIntParse() + o.Noballs.SmartIntParse()
                });
                // Byes and Leg Byes goes to team
                return ExtrasGiven.Where(s => s.Extras != 0 && s.Extras == ExtrasGiven.Max(o => o.Extras))
                            .Select(t => t.BowlingTeamId.ToString()).ToList();
            }
        }
        public List<string> TeamToScoreMaxRunsInPP //MAX_PP_TEAM
        {
            get
            {

                var RunsInPP = _MatchAnalyticsDoc.Descendants("Innings")
                                .Select(o => new
                                {
                                    Inning = o.Attribute("Number").Value,
                                    Runs = _MatchAnalyticsDoc.Descendants("Innings")
                                                .Where(i => i.Attribute("Number").Value == o.Attribute("Number").Value)
                                                .Select(r =>
                                                            r.Descendants("Node").Where(p => p.Attribute("IsPowerPlay").Value == "yes")
                                                                .Select(q => q.Descendants("BattingParameters")
                                                                            .Descendants("RunsScored").FirstOrDefault().Value.SmartIntParse()
                                                                                +
                                                                            q.Descendants("BowlingParameters")
                                                                            .Descendants("ExtrasConceded").FirstOrDefault().Value.SmartIntParse()
                                                                            )
                                                                .Aggregate((m, n) => m + n)).FirstOrDefault()
                                });

                return (from a in RunsInPP
                        join b in _MatchFeed.Innings
                            on a.Inning equals b.Number
                        where a.Runs == RunsInPP.Max(o => o.Runs)
                        select b.Battingteam).ToList();
            }
        }


        public int NoOfSixesInMatch //SIX_MATCH
        {
            get
            {
                return _MatchPlayerStats.PlayerStats.Sum(o => o.SixesHit);
            }
        }
        public int NoOfFoursInMatch//FOUR_MATCH
        {
            get
            {
                return _MatchPlayerStats.PlayerStats.Sum(o => o.FoursHit);
            }
        }
        public int NoOfWicketsInMatch //WCKT_MATCH
        {
            get
            {
                return _MatchFeed.Innings.Sum(o => o.Wickets.SmartIntParse());
            }
        }
        public int TotalRunsInMatch //RUN_MATCH
        {
            get
            {
                return _MatchFeed.Innings.Sum(o => o.Total.SmartIntParse());
            }
        }
        public int HighestScoreInMatch //HIG_SCOR_MATCH
        {
            get
            {

                return _MatchPlayerStats.PlayerStats.Max(o => o.RunsScored);
            }
        }
        public int TeamToWinMatch
        {
            get
            {
                return _MatchFeed.Matchdetail.Winningteam.SmartIntParse() == 0 ?
                    _Match == null ? 0 : _Match.winningteam_Id.SmartIntParse() : _MatchFeed.Matchdetail.Winningteam.SmartIntParse();
            }
        }

    }

    public class InningQuestions
    {
        private readonly MatchFeed _MatchFeed;
        private readonly XDocument _MatchAnalyticsDoc;
        private readonly string _Inning;

        private readonly string _BattingTeadId;
        private readonly string _BowlingTeadId;
        private readonly Innings _InningInfo;

        private readonly List<PlayerStats> _BowlingPlayerStats;
        private readonly List<PlayerStats> _BattingPlayerStats;

        public InningQuestions(MatchFeed vMatchFeed, MatchPlayerStats vMatchPlayerStats,
                                            XDocument vMatchAnalyticsDoc, string vInning)
        {
            _MatchFeed = vMatchFeed;
            _MatchAnalyticsDoc = vMatchAnalyticsDoc;

            _Inning = vInning.ToLower();
            _InningInfo = vMatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning).FirstOrDefault();

            _BattingTeadId = _InningInfo.Battingteam;
            _BowlingTeadId = vMatchFeed.Matchdetail.Team_Away == _BattingTeadId ?
                                    vMatchFeed.Matchdetail.Team_Home : vMatchFeed.Matchdetail.Team_Away;

            _BowlingPlayerStats = vMatchPlayerStats.PlayerStats.Where(o => o.TeamId.ToString() == _BowlingTeadId).Select(o => o).ToList();
            _BattingPlayerStats = vMatchPlayerStats.PlayerStats.Where(o => o.TeamId.ToString() == _BattingTeadId).Select(o => o).ToList();
        }

        public List<string> PlayerToHitMostSixes //SIX_PLYR
        {
            get
            {
                return _BattingPlayerStats
                            .Where(p => p.SixesHit != 0 && p.SixesHit == _BattingPlayerStats.Max(m => m.SixesHit))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<string> PlayerToHitMostFours//FOUR_PLYR
        {
            get
            {
                return _BattingPlayerStats
                            .Where(p => p.FoursHit != 0 && p.FoursHit == _BattingPlayerStats.Max(m => m.FoursHit))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<string> PlayerToTakeMostWickets //WCKT_TKN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.Wickets != 0 && p.Wickets == _BowlingPlayerStats.Max(m => m.Wickets))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<string> PlayerToTakeMostCatchs //CATCH_TKN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.Catches != 0 && p.Catches == _BowlingPlayerStats.Max(m => m.Catches))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<string> PlayerToConcedeMostExtras //EXTRA_GVN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.ExtrasNoWide != 0 && p.ExtrasNoWide == _BowlingPlayerStats.Max(m => m.ExtrasNoWide))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<string> PlayerToConcedeMostRuns //RUN_GVN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.RunsGiven != 0 && p.RunsGiven == _BowlingPlayerStats.Max(m => m.RunsGiven))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }


        public int WicketsInInning //WCKT_TKN_ING
        {
            get
            {
                return _MatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning).FirstOrDefault().Wickets.SmartIntParse();
            }
        }
        public int WicketsInPP //WCKT_TKN_PP
        {
            get
            {
                return _MatchAnalyticsDoc.Descendants("Innings")
                                    .Where(o => o.Attribute("Number").Value.ToLower() == _Inning).FirstOrDefault()
                                    .Descendants("Node").Where(n => n.Attribute("IsPowerPlay").Value == "yes")
                                    .Descendants("BattingParameters")
                                            .Where(d => string.IsNullOrEmpty(d.Descendants("Dismissal").Descendants("Batsman").FirstOrDefault().Value) == false)
                                    .Count();
            }
        }
        public int ExtrasInInning //EXTRA_TKN_ING
        {
            get
            {
                return _MatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning)
                            .Select(o => o.Noballs.SmartIntParse() + o.Wides.SmartIntParse()
                                        + o.Legbyes.SmartIntParse() + o.Byes.SmartIntParse()).FirstOrDefault();
            }
        }
        public int RunsInPP //RUN_PP
        {
            get
            {

                return _MatchAnalyticsDoc.Descendants("Innings")
                            .Where(o => o.Attribute("Number").Value.ToLower() == _Inning)
                            .Select(r => r.Descendants("Node").Where(p => p.Attribute("IsPowerPlay").Value == "yes")
                                .Select(q => q.Descendants("BattingParameters")
                                            .Descendants("RunsScored").FirstOrDefault().Value.SmartIntParse()
                                                +
                                            q.Descendants("BowlingParameters")
                                            .Descendants("ExtrasConceded").FirstOrDefault().Value.SmartIntParse()
                                            )
                                .Aggregate((m, n) => m + n)).FirstOrDefault();

            }
        }
        public int RunsInInning //RUN_ING
        {
            get
            {
                return _MatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning).FirstOrDefault().Total.SmartIntParse();
            }
        }
        public int RunsInLst5Overs { get; set; } //RUN_L5
    }
}
