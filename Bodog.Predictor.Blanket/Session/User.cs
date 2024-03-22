using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Session;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bodog.Predictor.Blanket.Session
{
    public class User : Common.BaseBlanket
    {

        private readonly DataAccess.Session.User _DBContext;
        private readonly Int32 _TourId;

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
            Int32 mUserId = 0;
            try
            {
                if (credentials.OptType == 2)
                {
                    if (_Cookies._HasUserCookies)
                    {
                        mUserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    }
                }

                if (credentials.OptType == 1 && credentials.EmailId == null)
                {
                    credentials.EmailId = String.Empty;
                }

                UserDetails details = _DBContext.Login(credentials.OptType, credentials.PlatformId, _TourId, mUserId, credentials.SocialId, credentials.ClientId,
                    credentials.FullName, credentials.EmailId, credentials.PhoneNo, credentials.CountryCode, credentials.ProfilePicture, ref httpMeta);

                if (httpMeta.RetVal == 1)
                {
                    if (details != null && details.User != null && details.Game != null)
                    {
                        bool success = _Cookies.SetUserCookies(details.User);
                        success = _Cookies.SetGameCookies(details.Game);

                        Int32 retVal = (success) ? 1 : -100;

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

        public HTTPResponse UserPhoneUpdate(Int32 platformId, Int32 clientId, Int64 phoneNumber)
        {
            HTTPResponse httpResponse = new HTTPResponse();
            HTTPMeta httpMeta = new HTTPMeta();
            try
            {
                if (_Cookies._HasUserCookies)
                {
                    Int32 UserId = Int32.Parse(_Cookies._GetUserCookies.UserId);
                    //Int32 UserTourTeamId = Int32.Parse(BareEncryption.BaseDecrypt(_Cookies._GetGameCookies.TeamId));
                    Int32 OptType = 1;
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
