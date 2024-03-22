using Microsoft.AspNetCore.Http;
using System;

namespace Bodog.Predictor.Admin.App_Code
{
    public class Session
    {
        private IHttpContextAccessor _HttpContextAccessor;
        public const string _AdminCookey = "Bodog_Samurai";
        private readonly int _ExpiryDays = 1;

        public Session(IHttpContextAccessor httpContextAccessor)
        {
            _HttpContextAccessor = httpContextAccessor;
        }

        public bool _HasAdminCookie
        {
            get
            {
                return (_HttpContextAccessor.HttpContext.Request.Cookies[_AdminCookey] != null);
            }
        }

        public bool SetAdminCookie(String value)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(_ExpiryDays);

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(_AdminCookey, value, option);

            return true;
        }

        public String SlideAdminCookie()
        {
            String value = _HttpContextAccessor.HttpContext.Request.Cookies[_AdminCookey];
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(_ExpiryDays);

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(_AdminCookey, value, option);

            return !String.IsNullOrEmpty(value) ? value : "";
        }

        public void DeleteAdminCookie()
        {
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_AdminCookey);
        }
    }
}