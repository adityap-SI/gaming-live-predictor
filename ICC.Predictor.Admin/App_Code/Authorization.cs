using ICC.Predictor.Contracts.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Admin.App_Code
{
    public class Authorization : Session, Interfaces.Admin.ISession
    {
        private readonly Contracts.Configuration.Admin _Admin;

        public Authorization(IHttpContextAccessor httpContextAccessor, IOptions<Application> appSettings) : base(httpContextAccessor)
        {
            _Admin = appSettings.Value.Admin;
        }

        public List<string> Pages(string name = "")
        {
            string user = name;

            if (string.IsNullOrEmpty(user))
                user = SlideAdminCookie();

            Contracts.Configuration.Authorization authority = _Admin.Authorization.Where(o => o.User.ToLower().Trim() == user.ToLower().Trim()).FirstOrDefault();

            List<string> pages = authority.Pages.ToList();

            return pages;
        }
    }
}
