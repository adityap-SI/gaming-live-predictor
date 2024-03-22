using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Library.Utility;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Linq;

namespace Bodog.Predictor.Library.Connection
{
    public class Redis : Interfaces.Connection.IRedis
    {
        private static Contracts.Configuration.Redis _ConnectionEnvironment;

        public Redis(IOptions<Application> appSettings)
        {
            _ConnectionEnvironment = appSettings.Value.Connection.Redis;
        }

        public void RedisConnectMultiplexer()
        {
            ConnectMultiplexer();
        }

        public void RedisConnectDisposer()
        {
            ConnectDisposer();
        }

        private static ConnectionMultiplexer _ClientManager;

        #region " Connection Managers "

        private static void ConnectMultiplexer()
        {
            EndPointCollection mEndPointCollection = new EndPointCollection();
            ConfigurationOptions mConfigurationOptions = new ConfigurationOptions();

            mConfigurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                KeepAlive = 60, // 60 sec to ensure connection is alive
                ConnectTimeout = 10000, // 10 sec
                SyncTimeout = 10000, // 10 sec
            };

            String mRedisConn = _ConnectionEnvironment.Server;

            foreach (String mStr in mRedisConn.Split(',').ToList())
            {
                mConfigurationOptions.EndPoints.Add(mStr.Trim(), _ConnectionEnvironment.Port);
            }

            _ClientManager = ConnectionMultiplexer.Connect(mConfigurationOptions);
        }

        private static void ConnectDisposer()
        {
            _ClientManager.Dispose();
        }

        #endregion " Connection Managers "

        #region " Get/Set/Remove/Check Exist "

        public String GetData(String key)
        {
            IDatabase mRedisClient;
            String mData = String.Empty;

            mRedisClient = _ClientManager.GetDatabase();

            try
            {
                DateTime mExpireOn = DateTime.UtcNow.AddYears(1);
                mRedisClient.KeyExpire(key, mExpireOn);

                RedisValue[] mRedData = mRedisClient.SetMembers(key);

                if (mRedData != null)
                    mData = mRedData[0].ToString();
            }
            catch (Exception ex)
            {
                mData = "";
            }

            return mData;
        }

        public bool SetData(String key, Object content, bool serialize)
        {
            IDatabase mRedisClient;
            String mData = String.Empty;
            bool mSuccess = false;

            String data = serialize ? GenericFunctions.Serialize(content) : content.ToString();

            mRedisClient = _ClientManager.GetDatabase();

            try
            {
                if (mRedisClient.KeyExists(key))
                    mRedisClient.KeyDelete(key);

                mSuccess = mRedisClient.SetAdd(key, data);
                //--
                DateTime mExpireOn = DateTime.UtcNow.AddYears(1);
                mRedisClient.KeyExpire(key, mExpireOn);
                //--
            }
            catch (Exception ex)
            {
                mSuccess = false;
            }

            return mSuccess;
        }

        public bool Delete(String key)
        {
            IDatabase mRedisClient;
            bool mSuccess = false;

            mRedisClient = _ClientManager.GetDatabase();

            if (mRedisClient.KeyExists(key))
            {
                try
                {
                    mRedisClient.KeyDelete(key);
                    mSuccess = true;
                }
                catch
                {
                    mSuccess = false;
                }
            }
            else
            {
                mSuccess = true;
            }

            return mSuccess;
        }

        public bool Has(String key)
        {
            IDatabase mRedisClient;

            mRedisClient = _ClientManager.GetDatabase();

            return mRedisClient.KeyExists(key);
        }

        #endregion " Get/Set/Remove/Check Exist "
    }
}