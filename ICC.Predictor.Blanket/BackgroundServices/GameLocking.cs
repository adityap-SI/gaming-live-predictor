using System;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using ICC.Predictor.Contracts.BackgroundServices;
using System.Net;
using System.Xml.Linq;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Blanket.Feeds;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Blanket.BackgroundServices
{
    public class GameLocking : BaseServiceBlanket
    {
        private readonly int _MatchLockMinutes;
        private readonly Gameplay _Feeds;
        private readonly DataAccess.BackgroundServices.GameLocking _Locking;
        private readonly int _TourId;
        private readonly string _CricketHostedApi;
        private readonly string _Client;
        private static string _ScoresFeed { get { return "type=scores"; } }
        private static string _AnalyticsFeed { get { return "type=analytics"; } }
        private static string _FixturesFeed { get { return "type=fixtures"; } }

        public GameLocking(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
          : base(appSettings, serviceSettings, aws, postgre, redis, cookies, asset)
        {
            //_MatchLockMinutes = serviceSettings.Value.GameLocking.MatchLockMinutes;
            _Feeds = new Gameplay(appSettings, aws, postgre, redis, cookies, asset);
            _Locking = new DataAccess.BackgroundServices.GameLocking(postgre);
            _TourId = appSettings.Value.Properties.TourId;
            _CricketHostedApi = appSettings.Value.Admin.Feed.API;
            _Client = $"client={appSettings.Value.Admin.Feed.Client}";
        }


        public int Lock(int optType, int matchId, int inningNo)
        {
            return _Locking.Lock(optType, _TourId, matchId, inningNo);
        }

        public int UnLock(int optType, int matchId, int inningNo)
        {
            return _Locking.UnLock(optType, _TourId, matchId, inningNo);
        }

        public int InsertMatchLineups(int optType, int matchId, List<Lineups> lineups)
        {
            List<string> skillName = new List<string>();
            List<int> skillId = new List<int>();
            for (int i = 0; i < lineups.Count; i++)
            {
                skillName.Add(string.Empty);
                skillId.Add(0);
            }
            return _Locking.InsertMatchLineups(optType, _TourId, matchId, lineups, skillName, skillId);
        }

        public int ProcessMatchToss(int optType, int matchId, int inningOneBatTeamId, int inningOneBowlTeamId, int inningTwoBatTeamId, int inningTwoBowlTeamId)
        {
            return _Locking.ProcessMatchToss(optType, _TourId, matchId, inningOneBatTeamId, inningOneBowlTeamId, inningTwoBatTeamId, inningTwoBowlTeamId);
        }

        public List<Fixtures> NextMatchList()
        {
            List<Fixtures> fixtures = new List<Fixtures>();

            try
            {
                string lang = "en";
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
                string lang = "en";
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

        public MatchFeed GetMatchScoresFeed(string MatchId)
        {
            string mData = string.Empty;
            MatchFeed mMatchFeed = new MatchFeed();
            try
            {
                string mURL = string.Format("{0}?{1}&{2}&{3}&{4}", _CricketHostedApi, _Client, _ScoresFeed, "id=" + MatchId, "accept=json");
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

            foreach (string mTeamId in vMatchFeed.Teams.Keys)
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

        public XDocument GetMatchAnalyticsFeed(string vMatchId)
        {
            XDocument mMatchDocument = new XDocument();
            string content = "";

            //mPath = "http://cricket.hosted.sportz.io/apis/getfeeds.aspx?client=aW50ZXJuYWwx&type=analytics&id=" + vMatchId;
            string mURL = string.Format("{0}?{1}&{2}&{3}", _CricketHostedApi, _Client, _AnalyticsFeed, "id=" + vMatchId);

            content = GenericFunctions.GetWebData(mURL);
            mMatchDocument = XDocument.Parse(content);

            return mMatchDocument;
        }

        public List<Match> GetFixturesFeed()
        {
            string mData = string.Empty;
            AllFixtures mAllFixtures = new AllFixtures();
            List<Match> mMatchesList = new List<Match>();
            try
            {
                string mURL = string.Format("{0}?{1}&{2}&{3}", _CricketHostedApi, _Client, _FixturesFeed, "accept=json");
                mData = GenericFunctions.GetWebData(mURL);
                mAllFixtures = GenericFunctions.Deserialize<AllFixtures>(mData);
                if (mAllFixtures != null && mAllFixtures.data != null)
                {
                    //if (mAllFixtures.data.matches != null && mAllFixtures.data.matches.Count > 0)
                    //    mMatchesList = mAllFixtures.data.matches.Where(i=>i.series_Id== _SeriesId).ToList();
                    if (mAllFixtures.data.matches != null && mAllFixtures.data.matches.Count > 0)
                        mMatchesList = mAllFixtures.data.matches;
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
