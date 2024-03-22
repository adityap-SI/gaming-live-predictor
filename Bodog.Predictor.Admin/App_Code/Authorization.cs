using Bodog.Predictor.Contracts.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Admin.App_Code
{
    public class Authorization : Session, Interfaces.Admin.ISession
    {
        private readonly Contracts.Configuration.Admin _Admin;

        public Authorization(IHttpContextAccessor httpContextAccessor, IOptions<Application> appSettings) : base(httpContextAccessor)
        {
            _Admin = appSettings.Value.Admin;
        }

        public List<String> Pages(String name = "")
        {
            String user = name;

            if (String.IsNullOrEmpty(user))
                user = this.SlideAdminCookie();

            Contracts.Configuration.Authorization authority = _Admin.Authorization.Where(o => o.User.ToLower().Trim() == user.ToLower().Trim()).FirstOrDefault();

            List<String> pages = authority.Pages.ToList();

            return pages;
        }
    }
}
