using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ICC.Predictor.Blanket.Leaderboard
{
    public class Leaderbaord : Common.BaseBlanket
    {
        private readonly DataAccess.Leaderboard.Leaderbaord _DBContext;
        private readonly Utility _Utility;
        private readonly int _TourId;
        private readonly DataAccess.Feeds.Gameplay _DBFeedContext;

        public Leaderbaord(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Leaderboard.Leaderbaord(postgre);
            _Utility = new Utility(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
            _DBFeedContext = new DataAccess.Feeds.Gameplay(postgre);
        }


        public async Task<HTTPResponse> GetUserRank(int OptType, int GameDayID, int PhaseId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            if (_Cookies._HasGameCookies)
            {
                int TeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    int UserId = int.Parse(_Cookies._GetUserCookies.UserId);
                    try
                    {
                        httpResponse.Data = _DBContext.UserRank(OptType, _TourId, UserId, TeamId, GameDayID, PhaseId, ref httpMeta);
                    }
                    catch (Exception ex)
                    {
                        HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.GetUserRank", ex.Message);
                        _AWS.AppendS3Logs(httpLog);
                    }
                }
                else
                {
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
                }
            }
            else
            {
                GenericFunctions.AssetMeta(-40, ref httpMeta, "TeamId is zero");
            }


            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetTopRank(int optType, int vPhaseId, int vGamedayId, int pageOneChunk, int pageChunk, int pageNo)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int top = 1000;

            try
            {
                int[] pagePoints = GenericFunctions.GetPagePoints(pageOneChunk, pageChunk, pageNo);
                int from = pagePoints[0], to = pagePoints[1];

                // If Requested Records Is More than ingested records then getting records from database
                if (from >= top || to >= top)
                {
                    ResponseObject responseObject = _DBContext.Top(optType, vPhaseId, vGamedayId, pageNo, top, _TourId, from, to, ref httpMeta);
                    httpResponse.Data = responseObject;
                }
                else
                {
                    string data = await _Asset.GET(_Asset.LeaderBoard(optType, vGamedayId, vPhaseId));

                    ResponseObject res = _Utility.FetchRecords(data, from, to);

                    httpResponse.Data = res;

                    int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Leaderboard.Leaderboard.GetTopRank", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> PlayedGamedays()
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject responseObject = new ResponseObject();
            int optType = 1;
            string lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            try
            {
                string data = await _Asset.GET(_Asset.Fixtures(lang));
                responseObject = GenericFunctions.Deserialize<ResponseObject>(data);
                fixtures = GenericFunctions.Deserialize<List<Fixtures>>(responseObject.Value.ToString());
                if (fixtures != null && fixtures.Any())
                {
                    fixtures = fixtures.Where(i => i.MatchStatus == 3).OrderByDescending(x => GenericFunctions.ToUSCulture(x.Date)).Select(o => o).ToList();

                    UserPlayedLB mUserPlayedLB = new UserPlayedLB();
                    mUserPlayedLB.PlayedGamedays = (from a in fixtures.AsEnumerable()
                                                    select new PlayedGamedays
                                                    {
                                                        GamedayId = a.TourGamedayId,
                                                        GamedayName = GenericFunctions.ToUSCulture(a.Date).ToString("MMMM d"),
                                                        Matchdate = a.Date

                                                    }).ToList().GroupBy(x => x.GamedayId).Select(x => x.First()).ToList();
                    mUserPlayedLB.PlayedPhase = (from a in fixtures.AsEnumerable()
                                                 select new PlayedPhase
                                                 {
                                                     PhaseId = a.phaseId,
                                                     PhaseName = "Week " + a.phaseId,
                                                 }).ToList().GroupBy(x => x.PhaseId).Select(x => x.First()).ToList();


                    //mUserPlayedLB.OverAll = "OverAll";
                    responseObject.Value = mUserPlayedLB;
                    responseObject.FeedTime = GenericFunctions.GetFeedTime();
                    GenericFunctions.AssetMeta(1, ref httpMeta, "Success");
                }


            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Leaderboard.Leaderboard.PlayedGamedays", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            httpResponse.Meta = httpMeta;
            httpResponse.Data = responseObject;
            return httpResponse;
        }

        public List<Fixtures> getFixtures()
        {
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            List<Fixtures> mFixtures = new List<Fixtures>();

            try
            {

                responseObject = _DBFeedContext.GetFixtures(1, _TourId, "en", ref httpMeta);
                mFixtures = (List<Fixtures>)responseObject.Value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mFixtures;
        }

        public int AdminLeaderBoard(int TopUsers, int LeaderBoardTypeId, int GamedayId, int PhaseId, out Reports leaderboard)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int PageNo = 1;
            int FromRowNo = 1;
            int ToRowNo = TopUsers; //100;
            int retVal;

            leaderboard = new Reports();

            try
            {
                if (LeaderBoardTypeId == 1)
                {
                    leaderboard = _DBContext.AdminLeaderBoard(LeaderBoardTypeId, PageNo, TopUsers, _TourId, 0, 0, FromRowNo, ToRowNo, ref httpMeta);
                }
                else if (LeaderBoardTypeId == 2)
                {
                    leaderboard = _DBContext.AdminLeaderBoard(LeaderBoardTypeId, PageNo, TopUsers, _TourId, 0, GamedayId, FromRowNo, ToRowNo, ref httpMeta);
                }
                else if (LeaderBoardTypeId == 3)
                {
                    leaderboard = _DBContext.AdminLeaderBoard(LeaderBoardTypeId, PageNo, TopUsers, _TourId, PhaseId, 0, FromRowNo, ToRowNo, ref httpMeta);
                }
            }
            catch (Exception ex)
            {
                httpMeta.Message = ex.Message;
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Leaderboard.AdminLeaderBoard", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            //httpResponse.Meta = httpMeta;
            return Convert.ToInt32(httpMeta.RetVal);
        }
    }
}
