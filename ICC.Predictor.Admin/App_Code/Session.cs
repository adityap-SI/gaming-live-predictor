using Microsoft.AspNetCore.Http;
using System;

namespace ICC.Predictor.Admin.App_Code
{
    public class Session
    {
        private IHttpContextAccessor _HttpContextAccessor;
        public const string _AdminCookey = "ICC_Samurai";
        private readonly int _ExpiryDays = 1;

        public Session(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

        public bool _HasAdminCookie
        {
            get
            {
                return _HttpContextAccessor.HttpContext.Request.Cookies[_AdminCookey] != null;
            }
        }

        public bool SetAdminCookie(string value)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(_ExpiryDays);

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(_AdminCookey, value, option);

            return true;
        }

        public string SlideAdminCookie()
        {
            string value = _HttpContextAccessor.HttpContext.Request.Cookies[_AdminCookey];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(_ExpiryDays);

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(_AdminCookey, value, option);

            return !string.IsNullOrEmpty(value) ? value : "";
        }

        public void DeleteAdminCookie()
        {
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_AdminCookey);
        }
    }
}