using System;

namespace Bodog.Predictor.Interfaces.Connection
{
    public interface IRedis
    {
        void RedisConnectMultiplexer();

        void RedisConnectDisposer();

        String GetData(String key);

        bool SetData(String key, Object content, bool serialize);

        bool Delete(String key);

        bool Has(String key);
    }
}