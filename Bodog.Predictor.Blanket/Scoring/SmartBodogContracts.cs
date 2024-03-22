using Bodog.Predictor.Contracts.Admin;
using Bodog.Predictor.Library.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Bodog.Predictor.Blanket.Scoring
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

        public List<String> TeamToHitMostSixes //SIX_TEAM
        {
            get
            {
                var SixesHit = _MatchPlayerStats.PlayerStats.GroupBy(o => o.TeamId).Select(s => new
                {
                    TeamId = s.First().TeamId,
                    SixesHit = s.Sum(_ => _.SixesHit).ToString().SmartIntParse()
                }).ToList();

                return SixesHit.Where(s => s.SixesHit != 0 && s.SixesHit == SixesHit.Max(o => o.SixesHit))
                        .Select(t => t.TeamId.ToString()).ToList();
            }
        }
        public List<String> TeamToHitMostFours//FOUR_TEAM
        {
            get
            {
                var FoursHit = _MatchPlayerStats.PlayerStats.GroupBy(o => o.TeamId).Select(s => new
                {
                    TeamId = s.First().TeamId,
                    FoursHit = s.Sum(_ => _.FoursHit).ToString().SmartIntParse()
                }).ToList();

                return FoursHit.Where(s => s.FoursHit != 0 && s.FoursHit == FoursHit.Max(o => o.FoursHit))
                        .Select(t => t.TeamId.ToString()).ToList();
            }
        }
        public List<String> TeamToConcedeMostWickets //WCKT_GVN_TEAM
        {
            get
            {
                var WicketsCon = _MatchFeed.Innings.Select(o => new { TeamId = o.Battingteam, Wickets = o.Wickets.SmartIntParse() });

                return WicketsCon.Where(s => s.Wickets != 0 && s.Wickets == WicketsCon.Max(o => o.Wickets))
                        .Select(t => t.TeamId.ToString()).ToList();
            }
        }
        public List<String> TeamToTakeMostWickets //WCKT_TKN_TEAM
        {
            get
            {
                var WicketsTaken = _MatchFeed.Innings.Select(o => new
                {
                    BowlingTeamId = (_MatchFeed.Matchdetail.Team_Home == o.Battingteam) ? _MatchFeed.Matchdetail.Team_Away : _MatchFeed.Matchdetail.Team_Home,
                    Wickets = o.Wickets.SmartIntParse()
                });

                return WicketsTaken.Where(s => s.Wickets != 0 && s.Wickets == WicketsTaken.Max(o => o.Wickets))
                        .Select(t => t.BowlingTeamId.ToString()).ToList();
            }
        }
        public List<String> TeamToConcedeMostExtras //EXTRA_GVN_TEAM
        {
            get
            {
                var ExtrasGiven = _MatchFeed.Innings.Select(o => new
                {
                    BowlingTeamId = (_MatchFeed.Matchdetail.Team_Home == o.Battingteam) ? _MatchFeed.Matchdetail.Team_Away : _MatchFeed.Matchdetail.Team_Home,
                    Extras = o.Byes.SmartIntParse() + o.Legbyes.SmartIntParse() + o.Wides.SmartIntParse() + o.Noballs.SmartIntParse()
                });
                // Byes and Leg Byes goes to team
                return ExtrasGiven.Where(s => s.Extras != 0 && s.Extras == ExtrasGiven.Max(o => o.Extras))
                            .Select(t => t.BowlingTeamId.ToString()).ToList();
            }
        }
        public List<String> TeamToScoreMaxRunsInPP //MAX_PP_TEAM
        {
            get
            {

                var RunsInPP = _MatchAnalyticsDoc.Descendants("Innings")
                                .Select(o => new
                                {
                                    Inning = o.Attribute("Number").Value,
                                    Runs = (_MatchAnalyticsDoc.Descendants("Innings")
                                                .Where(i => i.Attribute("Number").Value == o.Attribute("Number").Value)
                                                .Select(r =>
                                                            r.Descendants("Node").Where(p => p.Attribute("IsPowerPlay").Value == "yes")
                                                                .Select(q => q.Descendants("BattingParameters")
                                                                            .Descendants("RunsScored").FirstOrDefault().Value.SmartIntParse()
                                                                                +
                                                                            q.Descendants("BowlingParameters")
                                                                            .Descendants("ExtrasConceded").FirstOrDefault().Value.SmartIntParse()
                                                                            )
                                                                .Aggregate((m, n) => m + n)).FirstOrDefault())
                                });

                return (from a in RunsInPP
                        join b in _MatchFeed.Innings
                            on a.Inning equals b.Number
                        where a.Runs == RunsInPP.Max(o => o.Runs)
                        select b.Battingteam).ToList();
            }
        }


        public Int32 NoOfSixesInMatch //SIX_MATCH
        {
            get
            {
                return _MatchPlayerStats.PlayerStats.Sum(o => o.SixesHit);
            }
        }
        public Int32 NoOfFoursInMatch//FOUR_MATCH
        {
            get
            {
                return _MatchPlayerStats.PlayerStats.Sum(o => o.FoursHit);
            }
        }
        public Int32 NoOfWicketsInMatch //WCKT_MATCH
        {
            get
            {
                return _MatchFeed.Innings.Sum(o => o.Wickets.SmartIntParse());
            }
        }
        public Int32 TotalRunsInMatch //RUN_MATCH
        {
            get
            {
                return _MatchFeed.Innings.Sum(o => o.Total.SmartIntParse());
            }
        }
        public Int32 HighestScoreInMatch //HIG_SCOR_MATCH
        {
            get
            {

                return _MatchPlayerStats.PlayerStats.Max(o => o.RunsScored);
            }
        }
        public Int32 TeamToWinMatch
        {
            get
            {
                return (_MatchFeed.Matchdetail.Winningteam.SmartIntParse() == 0 ? 
                    (_Match == null ? 0 : _Match.winningteam_Id.SmartIntParse() ) : _MatchFeed.Matchdetail.Winningteam.SmartIntParse());
            }
        }

    }

    public class InningQuestions
    {
        private readonly MatchFeed _MatchFeed;
        private readonly XDocument _MatchAnalyticsDoc;
        private readonly String _Inning;

        private readonly String _BattingTeadId;
        private readonly String _BowlingTeadId;
        private readonly Innings _InningInfo;

        private readonly List<PlayerStats> _BowlingPlayerStats;
        private readonly List<PlayerStats> _BattingPlayerStats;

        public InningQuestions(MatchFeed vMatchFeed, MatchPlayerStats vMatchPlayerStats,
                                            XDocument vMatchAnalyticsDoc, String vInning)
        {
            _MatchFeed = vMatchFeed;
            _MatchAnalyticsDoc = vMatchAnalyticsDoc;

            _Inning = vInning.ToLower();
            _InningInfo = vMatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning).FirstOrDefault();

            _BattingTeadId = _InningInfo.Battingteam;
            _BowlingTeadId = (vMatchFeed.Matchdetail.Team_Away == _BattingTeadId) ?
                                    vMatchFeed.Matchdetail.Team_Home : vMatchFeed.Matchdetail.Team_Away;

            _BowlingPlayerStats = vMatchPlayerStats.PlayerStats.Where(o => o.TeamId.ToString() == _BowlingTeadId).Select(o => o).ToList();
            _BattingPlayerStats = vMatchPlayerStats.PlayerStats.Where(o => o.TeamId.ToString() == _BattingTeadId).Select(o => o).ToList();
        }

        public List<String> PlayerToHitMostSixes //SIX_PLYR
        {
            get
            {
                return _BattingPlayerStats
                            .Where(p => p.SixesHit != 0 && p.SixesHit == (_BattingPlayerStats.Max(m => m.SixesHit)))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<String> PlayerToHitMostFours//FOUR_PLYR
        {
            get
            {
                return _BattingPlayerStats
                            .Where(p => p.FoursHit != 0 && p.FoursHit == (_BattingPlayerStats.Max(m => m.FoursHit)))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<String> PlayerToTakeMostWickets //WCKT_TKN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.Wickets != 0 && p.Wickets == (_BowlingPlayerStats.Max(m => m.Wickets)))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<String> PlayerToTakeMostCatchs //CATCH_TKN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.Catches != 0 && p.Catches == (_BowlingPlayerStats.Max(m => m.Catches)))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<String> PlayerToConcedeMostExtras //EXTRA_GVN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.ExtrasNoWide != 0 && p.ExtrasNoWide == (_BowlingPlayerStats.Max(m => m.ExtrasNoWide)))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }
        public List<String> PlayerToConcedeMostRuns //RUN_GVN_PLYR
        {
            get
            {
                return _BowlingPlayerStats
                            .Where(p => p.RunsGiven != 0 && p.RunsGiven == (_BowlingPlayerStats.Max(m => m.RunsGiven)))
                            .Select(o => o.PlayerId.ToString()).ToList();
            }
        }


        public Int32 WicketsInInning //WCKT_TKN_ING
        {
            get
            {
                return _MatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning).FirstOrDefault().Wickets.SmartIntParse();
            }
        }
        public Int32 WicketsInPP //WCKT_TKN_PP
        {
            get
            {
                return _MatchAnalyticsDoc.Descendants("Innings")
                                    .Where(o => o.Attribute("Number").Value.ToLower() == _Inning).FirstOrDefault()
                                    .Descendants("Node").Where(n => n.Attribute("IsPowerPlay").Value == "yes")
                                    .Descendants("BattingParameters")
                                            .Where(d => String.IsNullOrEmpty(d.Descendants("Dismissal").Descendants("Batsman").FirstOrDefault().Value) == false)
                                    .Count();
            }
        }
        public Int32 ExtrasInInning //EXTRA_TKN_ING
        {
            get
            {
                return _MatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning)
                            .Select(o => o.Noballs.SmartIntParse() + o.Wides.SmartIntParse()
                                        + o.Legbyes.SmartIntParse() + o.Byes.SmartIntParse()).FirstOrDefault();
            }
        }
        public Int32 RunsInPP //RUN_PP
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
        public Int32 RunsInInning //RUN_ING
        {
            get
            {
                return _MatchFeed.Innings.Where(o => o.Number.ToLower() == _Inning).FirstOrDefault().Total.SmartIntParse();
            }
        }
        public Int32 RunsInLst5Overs { get; set; } //RUN_L5
    }
}
