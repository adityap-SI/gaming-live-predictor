using ICC.Predictor.Contracts.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace ICC.Predictor.Library.Dependency
{
    public class Authentication
    {
        private readonly RequestDelegate _Next;
        private string _Username { get; set; } = "byakuya";
        private string _Password { get; set; } = "kuchiki";
        private readonly string _Header;
        private readonly string _Backdoor;
        private readonly string _Domain;
        private readonly IHttpContextAccessor _HttpContext;

        public Authentication(IOptions<Application> appSettings, IHttpContextAccessor context)
        {
            _Header = appSettings.Value.API.Authentication.Header;
            _Backdoor = appSettings.Value.API.Authentication.Backdoor;
            _Domain = appSettings.Value.API.Domain;
            _HttpContext = context;
        }

        /*public Authentication(IOptions<Application> appSettings, RequestDelegate next)
        {
            _Next = next;
        }*/

        public bool Validate(string backdoor = null)
        {
            bool valid = false;

            if (backdoor == _Backdoor)
                return true;

            string referer = _HttpContext.HttpContext.Request.Headers["Referer"];
            string header = _HttpContext.HttpContext.Request.Headers["entity"];

            //The below commented lines are to allow the API calls from APPS.
            //if (String.IsNullOrEmpty(referer))
            //    return false;

            //if ((referer.ToLower().IndexOf(_Domain.ToLower()) > -1 && _Header == header)
            //    || referer.ToLower().Trim().IndexOf("index.html") > -1)
            //    valid = true;
            if (_Header == header || referer.ToLower().IndexOf(_Domain.ToLower()) > -1 || referer.ToLower().Trim().IndexOf("index.html") > -1)
                valid = true;

            return valid;
        }

        /*public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                //Extract credentials
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                if (username == _Username && password == _Password)
                {
                    await _Next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }
            else
            {
                // no authorization header
                context.Response.StatusCode = 401; //Unauthorized
                return;
            }
        }*/
    }
}