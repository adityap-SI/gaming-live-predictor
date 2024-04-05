using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Common
{
    public class BaseBlanket
    {
        protected readonly IOptions<Application> _AppSettings;
        protected readonly IAWS _AWS;
        protected readonly IPostgre _Postgre;
        protected readonly IRedis _Redis;
        protected readonly ICookies _Cookies;
        protected readonly IAsset _Asset;

        public BaseBlanket(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
        {
            _AppSettings = appSettings;
            _AWS = aws;
            _Postgre = postgre;
            _Redis = redis;
            _Cookies = cookies;
            _Asset = asset;
        }

        public async Task<List<string>> GetLanguages()
        {
            string data = await _Asset.GET(_Asset.Languages());
            ResponseObject res = GenericFunctions.Deserialize<ResponseObject>(data);
            List<string> lang = GenericFunctions.Deserialize<List<string>>(GenericFunctions.Serialize(res.Value));
            return lang;
        }

        public async Task<List<Skills>> GetSkills(string lang)
        {
            string data = await _Asset.GET(_Asset.Skills(lang));
            ResponseObject res = GenericFunctions.Deserialize<ResponseObject>(data);
            List<Skills> skills = GenericFunctions.Deserialize<List<Skills>>(GenericFunctions.Serialize(res.Value));
            return skills;
        }

        public async Task<string> DefaultLang(string lang)
        {
            List<string> languages = await GetLanguages();

            string data = languages.Any(o => o != null && lang != null && o.Trim().ToLower() == lang.ToLower()) ? lang : "en";

            return data;
        }
    }
}