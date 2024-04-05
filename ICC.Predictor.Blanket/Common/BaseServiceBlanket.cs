using Microsoft.Extensions.Options;
using ICC.Predictor.Interfaces.Session;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Asset;

namespace ICC.Predictor.Blanket.Common
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
