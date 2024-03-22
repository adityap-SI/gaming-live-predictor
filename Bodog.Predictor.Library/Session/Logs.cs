using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Library.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bodog.Predictor.Library.Session
{
    public class Logs
    {
        protected readonly IHttpContextAccessor _HttpContextAccessor;

        public Logs(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

        public HTTPLog PopulateLog(String FunctionName, String Message)
        {
            HTTPLog mHTTPLog = new HTTPLog();

            try
            {
                String userCookie = _HttpContextAccessor.HttpContext.Request.Cookies[""];
                String gameCookie = _HttpContextAccessor.HttpContext.Request.Cookies[""];

                mHTTPLog.Function = FunctionName;
                mHTTPLog.Message = Message;
                mHTTPLog.RequestType = _HttpContextAccessor.HttpContext.Request.Method;
                mHTTPLog.RequestUri = _HttpContextAccessor.HttpContext.Request.Path + Query(_HttpContextAccessor.HttpContext.Request.Query);
                mHTTPLog.UserAgent = _HttpContextAccessor.HttpContext.Request.Headers["User-Agent"];
                mHTTPLog.Timestamp = GenericFunctions.GetFeedTime();
                mHTTPLog.Cookies = new { UserCookie = userCookie, GameCookie = gameCookie };

                if (mHTTPLog.RequestType.ToUpper() == "GET")
                    mHTTPLog.Payload = Query(_HttpContextAccessor.HttpContext.Request.Query);
                else if (mHTTPLog.RequestType.ToUpper().ToUpper() == "POST")
                {
                    StreamReader reader = new StreamReader(_HttpContextAccessor.HttpContext.Request.Body, System.Text.Encoding.UTF8);
                    mHTTPLog.Payload = reader.ReadToEnd();
                }
            }
            catch { }

            return mHTTPLog;
        }

        private String Query(IQueryCollection query)
        {
            string value = "?";

            foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> dict in _HttpContextAccessor.HttpContext.Request.Query)
            {
                value += dict.Key + "=" + dict.Value + "&";
            }

            if (value == "?")
                value = "";

            return value;
        }
    }
}