using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Session;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICC.Predictor.Blanket.Session
{
    public class User : BaseBlanket
    {

        private readonly DataAccess.Session.User _DBContext;
        private readonly int _TourId;

        public User(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Session.User(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }


        public HTTPResponse Login(Credentials credentials)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            ResponseObject res = new ResponseObject();
            HTTPMeta httpMeta = new HTTPMeta();
            int mUserId = 0;
            try
            {
                if (credentials.OptType == 2)
                {
                    if (_Cookies._HasUserCookies)
                    {
                        mUserId = int.Parse(_Cookies._GetUserCookies.UserId);
                    }
                }

                if (credentials.OptType == 1 && credentials.EmailId == null)
                {
                    credentials.EmailId = string.Empty;
                }

                UserDetails details = _DBContext.Login(credentials.OptType, credentials.PlatformId, _TourId, mUserId, credentials.SocialId, credentials.ClientId,
                    credentials.FullName, credentials.EmailId, credentials.PhoneNo, credentials.CountryCode, credentials.ProfilePicture, ref httpMeta);

                if (httpMeta.RetVal == 1)
                {
                    if (details != null && details.User != null && details.Game != null)
                    {
                        bool success = _Cookies.SetUserCookies(details.User);
                        success = _Cookies.SetGameCookies(details.Game);

                        int retVal = success ? 1 : -100;

                        res.Value = retVal;
                        res.FeedTime = GenericFunctions.GetFeedTime();
                        httpResponse.Data = res;

                        GenericFunctions.AssetMeta(retVal, ref httpMeta);
                    }
                    else
                        GenericFunctions.AssetMeta(-40, ref httpMeta, "Details from database is NULL.");
                }
                else if (httpMeta.RetVal == 3)
                {
                    GenericFunctions.AssetMeta(httpMeta.RetVal, ref httpMeta, "Email id already exists.");
                }
                else
                    GenericFunctions.AssetMeta(httpMeta.RetVal, ref httpMeta, "Error while fetching user details from database.");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Session.User.Login", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }

            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

        public HTTPResponse UserPhoneUpdate(int platformId, int clientId, long phoneNumber)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                if (_Cookies._HasUserCookies)
                {
                    int UserId = int.Parse(_Cookies._GetUserCookies.UserId);
                    //Int32 UserTourTeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    int OptType = 1;
                    httpResponse.Data = _DBContext.UserPhoneUpdate(OptType, platformId, _TourId, UserId, clientId, phoneNumber, ref httpMeta);

                }
                else
                    GenericFunctions.AssetMeta(-40, ref httpMeta, "Not Authorized");
            }
            catch (Exception ex)
            {
                HTTPLog httpLog = _Cookies.PopulateLog("Blanket.Session.User.UserPhoneUpdate", ex.Message);
                _AWS.AppendS3Logs(httpLog);
            }
            httpResponse.Meta = httpMeta;
            return httpResponse;
        }

    }
}
