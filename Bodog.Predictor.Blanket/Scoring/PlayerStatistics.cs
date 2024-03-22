using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodog.Predictor.Contracts.Admin;
using System.Xml.Linq;

namespace Bodog.Predictor.Blanket.Scoring
{
    public class PlayerStatistics : Common.BaseBlanket
    {
        private readonly Int32 _TourId;

        public PlayerStatistics(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _TourId = appSettings.Value.Properties.TourId;
        }


        public MatchPlayerStats GetPlayerStats(MatchFeed vMatchFeed, List<Lineups> vMatchLineups)
        {
            MatchPlayerStats mMatchPlayerStats = new MatchPlayerStats();

            mMatchPlayerStats.MatchId = vMatchFeed.Matchdetail.Match.Code;

            mMatchPlayerStats.HomeTeamId = vMatchFeed.Matchdetail.Team_Home.SmartIntParse();
            mMatchPlayerStats.AwayTeamId = vMatchFeed.Matchdetail.Team_Away.SmartIntParse();
            mMatchPlayerStats.TossWonById = vMatchFeed.Matchdetail.Tosswonby.SmartIntParse();
            mMatchPlayerStats.Status = vMatchFeed.Matchdetail.Status;
            mMatchPlayerStats.WinningTeamId = vMatchFeed.Matchdetail.Winningteam.SmartIntParse();


            mMatchPlayerStats.PlayerStats = (from LineUps in vMatchLineups
                                             join BatsmanStats in (List<BatsmanStats>)vMatchFeed.Innings.SelectMany(o => o.Batsmen).ToList()
                                              on LineUps.PlayerId equals BatsmanStats.Batsman into Batsmen
                                             from BatsmanStats in Batsmen.DefaultIfEmpty()
                                             join BowlerStats in (List<BowlerStats>)vMatchFeed.Innings.SelectMany(o => o.Bowlers).ToList()
                                                 on LineUps.PlayerId equals BowlerStats.Bowler into Bowler
                                             from BowlerStats in Bowler.DefaultIfEmpty()
                                             select new PlayerStats
                                             {
                                                 PlayerId = Int64.Parse(LineUps.PlayerId),
                                                 PlayerName = LineUps.PlayerName,
                                                 TeamId = Int64.Parse(LineUps.TeamId),

                                                 #region " Batting Stats "

                                                 RunsScored = (BatsmanStats != null) ? BatsmanStats.Runs.SmartIntParse() : 0,
                                                 SixesHit = (BatsmanStats != null) ? BatsmanStats.Sixes.SmartIntParse() : 0,
                                                 FoursHit = (BatsmanStats != null) ? BatsmanStats.Fours.SmartIntParse() : 0,

                                                 #endregion

                                                 #region " Bowling Stats"

                                                 Wickets = (BowlerStats != null) ? BowlerStats.Wickets.SmartIntParse() : 0,
                                                 RunsGiven = (BowlerStats != null) ? BowlerStats.Runs.SmartIntParse() : 0,
                                                 WideBalls = (BowlerStats != null) ? BowlerStats.Wides.SmartIntParse() : 0,
                                                 NoBalls = (BowlerStats != null) ? BowlerStats.Noballs.SmartIntParse() : 0,

                                                 #endregion

                                                 #region " Fielding Stats "

                                                 Catches = vMatchFeed.Innings.SelectMany(o => o.Batsmen).ToList()
                                                             .Where(o => o.Dismissal == "caught" && o.Fielder == LineUps.PlayerId).ToList()
                                                             .Count()

                                                 #endregion

                                             }).ToList();


            return mMatchPlayerStats;
        }
    }
}
