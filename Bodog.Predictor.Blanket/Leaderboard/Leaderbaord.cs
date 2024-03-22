using Bodog.Predictor.Contracts.Admin;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bodog.Predictor.Blanket.Leaderboard
{
    public class Leaderbaord : Common.BaseBlanket
    {
        private readonly DataAccess.Leaderboard.Leaderbaord _DBContext;
        private readonly Utility _Utility;
        private readonly Int32 _TourId;
        private readonly DataAccess.Feeds.Gameplay _DBFeedContext;

        public Leaderbaord(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Leaderboard.Leaderbaord(postgre);
            _Utility = new Utility(appSettings, aws, postgre, redis, cookies, asset);
            _TourId = appSettings.Value.Properties.TourId;
            _DBFeedContext = new DataAccess.Feeds.Gameplay(postgre);
        }


        public async Task<HTTPResponse> GetUserRank(Int32 OptType, Int32 GameDayID, Int32 PhaseId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            if (_Cookies._HasGameCookies)
            {
                Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
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

        public async Task<HTTPResponse> GetTopRank(Int32 optType, Int32 vPhaseId, Int32 vGamedayId, Int32 pageOneChunk, Int32 pageChunk, Int32 pageNo)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 top = 1000;

            try
            {
                Int32[] pagePoints = GenericFunctions.GetPagePoints(pageOneChunk, pageChunk, pageNo);
                Int32 from = pagePoints[0], to = pagePoints[1];

                // If Requested Records Is More than ingested records then getting records from database
                if (from >= top || to >= top)
                {
                    ResponseObject responseObject = _DBContext.Top(optType, vPhaseId, vGamedayId, pageNo, top, _TourId, from, to, ref httpMeta);
                    httpResponse.Data = responseObject;
                }
                else
                {
                    String data = await _Asset.GET(_Asset.LeaderBoard(optType, vGamedayId, vPhaseId));

                    ResponseObject res = _Utility.FetchRecords(data, from, to);

                    httpResponse.Data = res;

                    Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

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
            Int32 optType = 1;
            String lang = "en";
            List<Fixtures> fixtures = new List<Fixtures>();
            try
            {
                String data = await _Asset.GET(_Asset.Fixtures(lang));
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
            catch(Exception ex)
            {
                throw ex;
            }
            return mFixtures;
        }

        public Int32 AdminLeaderBoard(Int32 TopUsers, Int32 LeaderBoardTypeId, Int32 GamedayId, Int32 PhaseId, out Reports leaderboard)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 PageNo = 1;
            Int32 FromRowNo = 1;
            Int32 ToRowNo = TopUsers; //100;
            Int32 retVal;

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
