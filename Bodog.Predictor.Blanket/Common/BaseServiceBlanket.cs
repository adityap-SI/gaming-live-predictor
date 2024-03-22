using Bodog.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Asset;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Session;

namespace Bodog.Predictor.Blanket.Common
{
    public class BaseServiceBlanket : BaseBlanket
    {
        protected readonly IOptions<Daemon> _ServiceSettings;

        public BaseServiceBlanket(IOptions<Application> appSettings, IOptions<Daemon> serviceSettings, IAWS aws, IPostgre postgre, IRedis redis,
            ICookies cookies, IAsset asset) : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _ServiceSettings = serviceSettings;
        }
    }
}
