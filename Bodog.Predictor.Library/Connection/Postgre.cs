using Bodog.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Bodog.Predictor.Library.Connection
{
    public class Postgre : Interfaces.Connection.IPostgre
    {
        private Contracts.Configuration.Postgre _conSettings;

        public Postgre(IOptions<Application> appSettings)
        {
            _conSettings = appSettings.Value.Connection.Postgre;
        }

        public String Schema { get { return _conSettings.Schema; } }

        public String ConnectionString
        {
            get
            {
                String p = "";
                String connection = _conSettings.Host;
                bool pooling = _conSettings.Pooling;
                int minPool = _conSettings.MinPoolSize;
                int maxPool = _conSettings.MaxPoolSize;

                if (pooling)
                    p = "Pooling=true;MinPoolSize=" + minPool + ";MaxPoolSize=" + maxPool + ";";
                else
                    p = "Pooling=false;";

                return String.Concat(connection, p);
            }
        }
    }
}