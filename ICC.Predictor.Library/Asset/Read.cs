using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ICC.Predictor.Library.Asset
{
    public class Read : Write
    {
        public Read(IAWS aws, IRedis redis, IOptions<Application> appSettings) : base(aws, redis, appSettings)
        {
        }

        public async Task<string> GET(string key)
        {
            string content = "";

            if (_UseRedis)
                content = _Redis.GetData(key);
            else
                content = await _AWS.ReadS3Asset(key);

            return content;
        }
    }
}