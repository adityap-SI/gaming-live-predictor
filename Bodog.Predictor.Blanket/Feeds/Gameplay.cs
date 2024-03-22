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

namespace Bodog.Predictor.Blanket.Feeds
{
    public class Gameplay : Common.BaseBlanket
    {
        private readonly DataAccess.Feeds.Gameplay _DBContext;
        private readonly Int32 _TourId;

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
                String data = await _Asset.GET(_Asset.Languages());

                httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

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

        public async Task<HTTPResponse> GetFixtures(String lang, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 optType = 1;

            try
            {
                if (offloadDb)
                {
                    String data = await _Asset.GET(_Asset.Fixtures(lang));

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

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

        public async Task<HTTPResponse> GetSkills(String lang, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 optType = 1;

            try
            {
                if (offloadDb)
                {
                    String data = await _Asset.GET(_Asset.Skills(lang));

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

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

        public async Task<HTTPResponse> GetQuestions(Int32? QuestionsMatchID, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 optType = 1;

            try
            {
                if (offloadDb)
                {
                    String data = await _Asset.GET(_Asset.MatchQuestions(QuestionsMatchID));

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

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
            Int32 optType = 1;

            try
            {
                if (offloadDb)
                {
                    String data = await _Asset.GET(_Asset.RecentResult());

                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);

                    Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;

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

        public async Task<HTTPResponse> MatchInningStatus(Int32 MatchId, bool offloadDb = true)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            ResponseObject responseObject = new ResponseObject();
            Int32 optType = 1;
            try
            {
                if (offloadDb)
                {
                    String data = await _Asset.GET(_Asset.MatchInningStatus(MatchId));
                    httpResponse.Data = GenericFunctions.Deserialize<ResponseObject>(data);
                    Int32 retVal = httpResponse.Data != null && httpResponse.Data.ToString() != "" ? 1 : -40;
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

        public async Task<HTTPResponse> GetUserPredictions(Int32 MatchID, Int32 GameDayID)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
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

        public async Task<HTTPResponse> GetUserProfile(Int32 PlatformId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
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
            Int32 optType = 1;
            String lang = "en";
            List<Fixtures> mFixtures = new List<Fixtures>();
            List<CurrentGamedayMatches> mCurrentGamedayMatches = new List<CurrentGamedayMatches>();
            try
            {
                String data = await _Asset.GET(_Asset.CurrentGamedayMatches());

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

        public async Task<HTTPResponse> GetOtherUserPredictions(Int32 MatchID, Int32 GameDayID, String UserId, String UserTeamId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            Int32 OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                //Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                if (_Cookies._HasUserCookies)
                {
                    //Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    try
                    {
                        Int32 userId = Int32.Parse(Encryption.BaseDecrypt(UserId));
                        Int32 teamId = Int32.Parse(Encryption.BaseDecrypt(UserTeamId));

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
            Int32 OptType = 1;

            if (_Cookies._HasGameCookies)
            {
                Int32 TeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
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

        public HTTPResponse UserPrediction(Int32 MatchId, Int32 TourGamedayId, Int32 QuestionId, Int32 OptionId, Int32 PlatformId)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                if (_Cookies._HasUserCookies)
                {
                    Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    Int32 UserTourTeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    Int32 OptType = 1;


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
