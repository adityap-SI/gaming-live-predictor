using System;

namespace ICC.Predictor.Interfaces.Connection
{
    public interface IRedis
    {
        void RedisConnectMultiplexer();

        void RedisConnectDisposer();

        string GetData(string key);

        bool SetData(string key, object content, bool serialize);

        bool Delete(string key);

        bool Has(string key);
    }
}