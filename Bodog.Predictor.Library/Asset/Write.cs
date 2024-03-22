using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.AWS;
using Bodog.Predictor.Interfaces.Connection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Bodog.Predictor.Library.Asset
{
    public class Write
    {
        protected readonly IAWS _AWS;
        protected readonly IRedis _Redis;
        protected readonly bool _UseRedis;
        protected readonly Int32 _TourId;
        protected readonly String _Connection;

        public Write(IAWS aws, IRedis redis, IOptions<Application> appSettings)
        {
            _AWS = aws;
            _Redis = redis;
            _UseRedis = appSettings.Value.Connection.Redis.Apply;
            _TourId = appSettings.Value.Properties.TourId;
            _Connection = appSettings.Value.Connection.Environment;
        }

        public async Task<bool> SET(String key, Object content, bool serialize = true)
        {
            bool success = false;

            if (_UseRedis)
                success = _Redis.SetData(key, content, serialize);
            else
                success = await _AWS.WriteS3Asset(key, content, serialize);

            return success;
        }
    }
}