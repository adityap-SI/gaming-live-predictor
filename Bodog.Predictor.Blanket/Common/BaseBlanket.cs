using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Session;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Blanket.Common
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

        public async Task<List<String>> GetLanguages()
        {
            String data = await _Asset.GET(_Asset.Languages());
            ResponseObject res = GenericFunctions.Deserialize<ResponseObject>(data);
            List<String> lang = GenericFunctions.Deserialize<List<String>>(GenericFunctions.Serialize(res.Value));
            return lang;
        }

        public async Task<List<Skills>> GetSkills(String lang)
        {
            String data = await _Asset.GET(_Asset.Skills(lang));
            ResponseObject res = GenericFunctions.Deserialize<ResponseObject>(data);
            List<Skills> skills = GenericFunctions.Deserialize<List<Skills>>(GenericFunctions.Serialize(res.Value));
            return skills;
        }

        public async Task<String> DefaultLang(String lang)
        {
            List<String> languages = await GetLanguages();

            String data = languages.Any(o => (o != null && lang != null) && o.Trim().ToLower() == lang.ToLower()) ? lang : "en";

            return data;
        }
    }
}