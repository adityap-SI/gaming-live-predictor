using System;
using System.Linq;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Common;
using Microsoft.Extensions.Options;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Contracts.Feeds;
using System.Collections.Generic;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Bodog.Predictor.Contracts.BackgroundServices;
using Bodog.Predictor.Contracts.Admin;
using System.Net;
using System.Xml.Linq;

namespace Bodog.Predictor.Blanket.BackgroundServices
{
    public class GameLocking : Common.BaseServiceBlanket
    {
        private readonly Int32 _MatchLockMinutes;
        private readonly Feeds.Gameplay _Feeds;
        private readonly DataAccess.BackgroundServices.GameLocking _Locking;
        private readonly Int32 _TourId;
        private readonly string _CricketHostedApi;
        private readonly string _Client;
        private static string _ScoresFeed { get { return "type=scores"; } }
        private static string _AnalyticsFeed { get { return "type=analytics"; } }
        private static string _FixturesFeed { get { return "type=fixtures"; } }

        public GameLocking(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            //_MatchLockMinutes = serviceSettings.Value.GameLocking.MatchLockMinutes;
            _Feeds = new Feeds.Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Locking = new DataAccess.BackgroundServices.GameLocking(postgre);
            _TourId = appSettings.Value.Properties.TourId;
            _CricketHostedApi = appSettings.Value.Admin.Feed.API;
            _Client = $"client={appSettings.Value.Admin.Feed.Client}";
        }


        public Int32 Lock(Int32 optType, Int32 matchId, Int32 inningNo)
        {
            return _Locking.Lock(optType, _TourId, matchId, inningNo);
        }

        public Int32 UnLock(Int32 optType, Int32 matchId, Int32 inningNo)
        {
            return _Locking.UnLock(optType, _TourId, matchId, inningNo);
        }

        public Int32 InsertMatchLineups(Int32 optType, Int32 matchId, List<Lineups> lineups)
        {
            List<string> skillName = new List<string>();
            List<int> skillId = new List<int>();
            for (int i = 0; i < lineups.Count; i++)
            {
                skillName.Add(String.Empty);
                skillId.Add(0);
            }
            return _Locking.InsertMatchLineups(optType, _TourId, matchId, lineups, skillName, skillId);
        }

        public Int32 ProcessMatchToss(Int32 optType, Int32 matchId, Int32 inningOneBatTeamId, Int32 inningOneBowlTeamId, Int32 inningTwoBatTeamId, Int32 inningTwoBowlTeamId)
        {
            return _Locking.ProcessMatchToss(optType, _TourId, matchId, inningOneBatTeamId, inningOneBowlTeamId, inningTwoBatTeamId, inningTwoBowlTeamId);
        }

        public List<Fixtures> NextMatchList()
        {
            List<Fixtures> fixtures = new List<Fixtures>();

            try
            {
                String lang = "en";
                HTTPResponse httpResponse = _Feeds.GetFixtures(lang).Result;

                if (httpResponse.Meta.RetVal == 1)
                {
                    if (httpResponse.Data != null)
                        fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                }
                else
                    throw new Exception("GetFixtures RetVal is not 1.");

                if (fixtures != null && fixtures.Any())
                {
                    //Fetch all open matches. Not considering Live matches.
                    //fixtures = fixtures.Where(i => i.MatchStatus != 3 && i.MatchStatus != 0).OrderBy(x => GenericFunctions.ToUSCulture(x.Deadlinedate)).Select(o => o).ToList();
                    fixtures = fixtures.Where(i => i.MatchStatus == 1).OrderBy(x => GenericFunctions.ToUSCulture(x.Deadlinedate)).Select(o => o).ToList();

                    if (fixtures != null && fixtures.Any())
                    {
                        DateTime latestUpcoming = GenericFunctions.ToUSCulture(fixtures[0].Deadlinedate);

                        //If 2 or more matches at same time, than fetch both, else return only the first.
                        List<Fixtures> sameDateFixtures = fixtures.Where(o => DateTime.Compare(GenericFunctions.ToUSCulture(o.Deadlinedate), latestUpcoming) == 0).DefaultIfEmpty(fixtures[0]).Select(i => i).ToList();

                        //Locking this fixtures
                        fixtures = sameDateFixtures;

                    }
                    else
                        throw new Exception("No matches with MatchStatus == 1");
                }
                else
                    throw new Exception("Fixtures is either - NULL OR Has matches with no data.");
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServices.GameLocking.NextMatchList: " + ex.Message);
            }

            return fixtures;
        }

        public List<Fixtures> LiveMatchList()
        {
            List<Fixtures> fixtures = new List<Fixtures>();

            try
            {
                String lang = "en";
                HTTPResponse httpResponse = _Feeds.GetFixtures(lang).Result;

                if (httpResponse.Meta.RetVal == 1)
                {
                    if (httpResponse.Data != null)
                        fixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(((ResponseObject)httpResponse.Data).Value));
                }
                else
                    throw new Exception("GetFixtures RetVal is not 1.");

                if (fixtures != null && fixtures.Any())
                {
                    //Fetch all non-locked and open matches. Not considering Live matches as well.
                    //fixtures = fixtures.Where(i => i.MatchStatus != 3 && i.MatchStatus != 0).Select(o => o).ToList();
                    fixtures = fixtures.Where(i => i.MatchStatus == 2).Select(o => o).ToList();

                    //if (fixtures == null && !fixtures.Any())
                    //{
                    //    throw new Exception("No matches with MatchStatus == 1");
                    //}
                }
                else
                    throw new Exception("Fixtures is either - NULL OR Has matches with no data.");
            }
            catch (Exception ex)
            {
                throw new Exception("Blanket.BackgroundServices.GameLocking.NextMatchList: " + ex.Message);
            }

            return fixtures;
        }


        #region " Scores "

        public MatchFeed GetMatchScoresFeed(String MatchId)
        {
            String mData = String.Empty;
            MatchFeed mMatchFeed = new MatchFeed();
            try
            {
                String mURL = String.Format("{0}?{1}&{2}&{3}&{4}", _CricketHostedApi, _Client, _ScoresFeed, "id=" + MatchId, "accept=json");
                mData = GenericFunctions.GetWebData(mURL);
                mMatchFeed = GenericFunctions.Deserialize<MatchFeed>(mData);
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mMatchFeed;
        }

        public List<Lineups> GetLineupsFromMatchFeed(MatchFeed vMatchFeed)
        {
            List<Lineups> mMatchLineup = new List<Lineups>();

            foreach (String mTeamId in vMatchFeed.Teams.Keys)
            {
                mMatchLineup.AddRange((from p in vMatchFeed.Teams[mTeamId].Players
                                       select new Lineups
                                       {
                                           TeamId = mTeamId,
                                           PlayerId = p.Key.ToString(),
                                           PlayerName = p.Value.Name_Full.Trim()
                                       }).ToList());
            }

            return mMatchLineup;
        }

        public XDocument GetMatchAnalyticsFeed(String vMatchId)
        {
            XDocument mMatchDocument = new XDocument();
            String content = "";

            //mPath = "http://cricket.hosted.sportz.io/apis/getfeeds.aspx?client=aW50ZXJuYWwx&type=analytics&id=" + vMatchId;
            String mURL = String.Format("{0}?{1}&{2}&{3}", _CricketHostedApi, _Client, _AnalyticsFeed, "id=" + vMatchId);

            content = GenericFunctions.GetWebData(mURL);
            mMatchDocument = XDocument.Parse(content);

            return mMatchDocument;
        }

        public List<Match> GetFixturesFeed()
        {
            String mData = String.Empty;
            AllFixtures mAllFixtures = new AllFixtures();
            List<Match> mMatchesList = new List<Match>();
            try
            {
                String mURL = String.Format("{0}?{1}&{2}&{3}", _CricketHostedApi, _Client, _FixturesFeed, "accept=json");
                mData = GenericFunctions.GetWebData(mURL);
                mAllFixtures = GenericFunctions.Deserialize<AllFixtures>(mData);
                if (mAllFixtures != null && mAllFixtures.data != null)
                {
                    //if (mAllFixtures.data.matches != null && mAllFixtures.data.matches.Count > 0)
                    //    mMatchesList = mAllFixtures.data.matches.Where(i=>i.series_Id== _SeriesId).ToList();
                    if (mAllFixtures.data.matches != null && mAllFixtures.data.matches.Count > 0)
                        mMatchesList =  mAllFixtures.data.matches;
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mMatchesList;
        }
        #endregion

    }
}
