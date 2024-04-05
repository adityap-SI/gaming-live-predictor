using ICC.Predictor.Blanket.Common;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Feeds
{
    public class Gameplay : BaseBlanket
    {
        private readonly DataAccess.Feeds.Gameplay _DBContext;
        private readonly int _TourId;

        public Gameplay(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Feeds.Gameplay(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        #region " GET "

        public async Task<HTTPResponse> GetLanguages(bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();

            try
            {
                string data = await _Asset.GET(_Asset.Languages());

                httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetLanguages", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetFixtures(string lang, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int optType = 1;

            try
            {
                if (offloadDb)
                {
                    string data = await _Asset.GET(_Asset.Fixtures(lang));

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
                else
                    httpResponse.Data = _DBContext.GetFixtures(optType, _TourId, lang, ref httpMeta);
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetFixtures", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetSkills(string lang, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int optType = 1;

            try
            {
                if (offloadDb)
                {
                    string data = await _Asset.GET(_Asset.Skills(lang));

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
                else
                    httpResponse.Data = _DBContext.GetSkills(optType, lang, ref httpMeta);
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetPlayers", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetQuestions(int? QuestionsMatchID, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int optType = 1;

            try
            {
                if (offloadDb)
                {
                    string data = await _Asset.GET(_Asset.MatchQuestions(QuestionsMatchID));

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
                else
                    httpResponse.Data = _DBContext.GetQuestions(optType, _TourId, QuestionsMatchID, ref httpMeta);
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetQuestions", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetRecentResults(bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int optType = 1;

            try
            {
                if (offloadDb)
                {
                    string data = await _Asset.GET(_Asset.RecentResult());

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
                else
                    httpResponse.Data = _DBContext.GetRecentResults(optType, _TourId, ref httpMeta);
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetRecentResults", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> MatchInningStatus(int MatchId, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject responseObject = new ResponseObject();
            int optType = 1;
            try
            {
                if (offloadDb)
                {
                    string data = await _Asset.GET(_Asset.MatchInningStatus(MatchId));
                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);
                    int retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;
                    GenericFunctions.AssetMeta(retVal, ref httpMeta, "Success");
                }
                else
                    httpResponse.Data = _DBContext.GetMatchInningStatus(optType, _TourId, MatchId, ref httpMeta);
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetMatchFixturesStatus", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public async Task<HTTPResponse> GetUserPredictions(int MatchID, int GameDayID)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                int TeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    int UserId = int.Parse(_Cookies._GetUserCookies.UserId);
                    try
                    {
                        httpResponse.Data = _DBContext.GetPredictions(OptType, _TourId, UserId, TeamId, MatchID, GameDayID, ref httpMeta);
                    }
                    catch (Exception ex)
                    {

                        HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetUserPredictions", ex.Message);
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

        public async Task<HTTPResponse> GetUserProfile(int PlatformId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                int TeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    int UserId = int.Parse(_Cookies._GetUserCookies.UserId);
                    try
                    {
                        httpResponse.Data = _DBContext.GetUserProfile(OptType, _TourId, UserId, TeamId, PlatformId, ref httpMeta);

                        //responseObject =  _DBContext.GetFixtures(1, _TourId, "en", ref httpMeta);
                        //List<Fixtures> mFixtures = new List<Fixtures>();
                        //if (httpResponse.Data != null)
                        //    mFixtures = GenericFunctions.Deserialize<List<Fixtures>>(GenericFunctions.Serialize(responseObject.Value));


                        //UserProfile userProfile = new UserProfile();
                        //responseObject = _DBContext.GetUserProfile(OptType, _TourId, UserId, TeamId, PlatformId, ref httpMeta);

                        //userProfile =  GenericFunctions.Deserialize<UserProfile>(GenericFunctions.Serialize(responseObject.Value));


                        //foreach(UserMatchData mUserMatchData in userProfile.UserMatchDataList)
                        //{
                        //    mUserMatchData.Date = "2/24/19 7:00:00 PM";
                        //}

                        //responseObject.Value = userProfile;
                        //httpResponse.Data = responseObject;


                    }
                    catch (Exception ex)
                    {

                        HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetUserProfile", ex.Message);
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

        public async Task<HTTPResponse> GetCurrentGamedayMatches()
        {
            HTTPResponse httpResponse = new HTTPResponse();
            ResponseObject responseObject = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            int optType = 1;
            string lang = "en";
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<CurrentGamedayMatches> mCurrentGamedayMatches = new List<CurrentGamedayMatches>();
            try
            {
                string data = await _Asset.GET(_Asset.CurrentGamedayMatches());

                responseObject = GenericFunctions.Deserialize<ResponseObject>(data);
                mCurrentGamedayMatches = GenericFunctions.Deserialize<List<CurrentGamedayMatches>>(responseObject.Value.ToString());
                responseObject.Value = mCurrentGamedayMatches;

                GenericFunctions.AssetMeta(1, ref httpMeta, "Success");

                httpResponse.Data = responseObject;
                httpResponse.Meta = httpMeta;

            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetCurrentGamedayMatches", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            return httpResponse;
        }

        public async Task<HTTPResponse> GetOtherUserPredictions(int MatchID, int GameDayID, string UserId, string UserTeamId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                //Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    //Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    try
                    {
                        int userId = int.Parse(Encryption.BaseDecrypt(UserId));
                        int teamId = int.Parse(Encryption.BaseDecrypt(UserTeamId));

                        httpResponse.Data = _DBContext.GetOtherUserPredictions(OptType, _TourId, userId, teamId, MatchID, GameDayID, ref httpMeta);
                    }
                    catch (Exception ex)
                    {

                        HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetOtherUserPredictions", ex.Message);
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

        public async Task<HTTPResponse> GetGamePlays()
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            int OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                int TeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    //Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    try
                    {
                        httpResponse.Data = _DBContext.GetGamePlays(OptType, _TourId, TeamId, ref httpMeta);
                    }
                    catch (Exception ex)
                    {

                        HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.GetOtherUserPredictions", ex.Message);
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

        #endregion " GET "

        #region " POST "

        public HTTPResponse UserPrediction(int MatchId, int TourGamedayId, int QuestionId, int OptionId, int PlatformId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                if (_Cookies._HasUserCookies)
                {
                    int UserId = int.Parse(_Cookies._GetUserCookies.UserId);
                    int UserTourTeamId = int.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    int OptType = 1;


                    if (UserTourTeamId != 0)
                    {
                        httpResponse.Data = _DBContext.UserPrediction(OptType, _TourId, UserId
                                            , UserTourTeamId, MatchId, TourGamedayId, QuestionId
                                            , OptionId, PlatformId, ref httpMeta);
                    }
                    else
                        GenericFunctions.AssetMeta(-40, ref httpMeta, "TeamId is zero");
                }
                else
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Feeds.Gameplay.UserPrediction", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        #endregion " POST "

    }
}
