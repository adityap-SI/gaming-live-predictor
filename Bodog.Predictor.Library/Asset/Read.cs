using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Bodog.Predictor.Library.Asset
{
    public class Read : Write
    {
        public Read(IAWS aws, IRedis redis, IOptions<Application> appSettings) : base(aws, redis, appSettings)
        {
        }

        public async Task<String> GET(String key)
        {
            String content = "";

            if (_UseRedis)
                content = _Redis.GetData(key);
            else
                content = await _AWS.ReadS3Asset(key);

            return content;
        }
    }
}