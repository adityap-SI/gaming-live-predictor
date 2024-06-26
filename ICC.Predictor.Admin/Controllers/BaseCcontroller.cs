﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Admin;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ICC.Predictor.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IOptions<Application> _AppSettings;
        protected readonly Contracts.Configuration.Admin _Admin;
        protected readonly ISession _Session;
        protected readonly IAWS _AWS;
        protected readonly IPostgre _Postgre;
        protected readonly ICookies _Cookies;
        protected readonly IRedis _Redis;
        protected readonly IAsset _Asset;

        public BaseController(IOptions<Application> appSettings, ISession session, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
        {
            _AppSettings = appSettings;
            _Admin = appSettings.Value.Admin;
            _Session = session;
            _AWS = aws;
            _Postgre = postgre;
            _Cookies = cookies;
            _Redis = redis;
            _Asset = asset;
        }
    }
}
