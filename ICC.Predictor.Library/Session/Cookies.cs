using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Session;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace ICC.Predictor.Library.Session
{
    public class Cookies : Logs, ICookies
    {
        #region " PROPERTIES "

        public readonly string _UserCookey;
        public readonly string _GameCookey;
        private readonly int _ExpiryDays;
        private readonly string _Domain;

        public Cookies(IHttpContextAccessor httpContextAccessor, IOptions<Application> appSettings) : base(httpContextAccessor)
        {
            _UserCookey = appSettings.Value.Properties.ClientName + "_007";
            _GameCookey = appSettings.Value.Properties.ClientName + "_RAW";
            _ExpiryDays = appSettings.Value.Cookies.ExpiryDays;
            _Domain = appSettings.Value.Cookies.Domain;
        }

        public bool _HasUserCookies
        {
            get
            {
                return _HttpContextAccessor.HttpContext.Request.Cookies[_UserCookey] != null;
            }
        }

        public UserCookie _GetUserCookies
        {
            get
            {
                return GetUserCookies();
            }
        }

        public bool _HasGameCookies
        {
            get
            {
                return _HttpContextAccessor.HttpContext.Request.Cookies[_GameCookey] != null;
            }
        }

        public GameCookie _GetGameCookies
        {
            get
            {
                return GetGameCookies();
            }
        }

        #endregion " PROPERTIES "

        #region " FUNCTIONS "

        #region " User Cookie "

        private UserCookie GetUserCookies()
        {
            string cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_UserCookey];

            if (!string.IsNullOrEmpty(cookie))
            {
                UserCookie uc = new UserCookie();

                try
                {
                    string DecryptedCookie = Encryption.BaseDecrypt(cookie);
                    uc = GenericFunctions.Deserialize<UserCookie>(DecryptedCookie);
                }
                catch { }

                return uc;
            }
            else
                return null;
        }

        public bool SetUserCookies(UserCookie uc)
        {
            bool set = false;

            try
            {
                string serializedCookie = GenericFunctions.Serialize(uc);
                string value = Encryption.BaseEncrypt(serializedCookie);

                SET(_UserCookey, value);

                set = true;
            }
            catch { }

            return set;
        }

        /*public bool UpdateUserCookies(UserCookie values)
        {
            bool set = false;
            try
            {
                UserCookie uc = new UserCookie();
                uc = GetUserCookies();

                if (uc != null)
                {
                    if (values != null && !String.IsNullOrEmpty(values.UserId))
                        uc.UserId = values.UserId;

                    set = SetUserCookies(uc);
                }
            }
            catch { }

            return set;
        }*/

        #endregion " User Cookie "

        #region " Game Cookie "

        private GameCookie GetGameCookies()
        {
            string cookie = _HttpContextAccessor.HttpContext.Request.Cookies[_GameCookey];

            if (!string.IsNullOrEmpty(cookie))
            {
                GameCookie gc = new GameCookie();

                try
                {
                    gc = GenericFunctions.Deserialize<GameCookie>(cookie);
                }
                catch { }

                return gc;
            }
            else
                return null;
        }

        public bool SetGameCookies(GameCookie gc)
        {
            bool set = false;

            try
            {
                string value = GenericFunctions.Serialize(gc);

                SET(_GameCookey, value);

                set = true;
            }
            catch { }

            return set;
        }

        /*public bool UpdateGameCookies(GameCookie values)
        {
            bool set = false;
            try
            {
                GameCookie gc = new GameCookie();
                gc = GetGameCookies();

                if (gc != null)
                {
                    if (values != null && !String.IsNullOrEmpty(values.GUID))
                        gc.GUID = values.GUID;

                    if (values != null && !String.IsNullOrEmpty(values.FullName))
                        gc.FullName = values.FullName;

                    set = SetGameCookies(gc);
                }
            }
            catch { }

            return set;
        }*/

        #endregion " Game Cookie "

        private void SET(string key, string value)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(_ExpiryDays);
            option.Domain = _Domain;

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }

        private void DELETE()
        {
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_UserCookey);
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(_GameCookey);
        }

        #endregion " FUNCTIONS "
    }
}