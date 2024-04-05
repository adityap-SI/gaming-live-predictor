using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ICC.Predictor.Library.Asset
{
    public class Write
    {
        protected readonly IAWS _AWS;
        protected readonly IRedis _Redis;
        protected readonly bool _UseRedis;
        protected readonly int _TourId;
        protected readonly string _Connection;

        public Write(IAWS aws, IRedis redis, IOptions<Application> appSettings)
        {
            _AWS = aws;
            _Redis = redis;
            _UseRedis = appSettings.Value.Connection.Redis.Apply;
            _TourId = appSettings.Value.Properties.TourId;
            _Connection = appSettings.Value.Connection.Environment;
        }

        public async Task<bool> SET(string key, object content, bool serialize = true)
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