using ICC.Predictor.Contracts.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace ICC.Predictor.Library.Connection
{
    public class Postgre : Interfaces.Connection.IPostgre
    {
        private Contracts.Configuration.Postgre _conSettings;

        public Postgre(IOptions<Application> appSettings)
        {
            _conSettings = appSettings.Value.Connection.Postgre;
        }

        public string Schema { get { return _conSettings.Schema; } }

        public string ConnectionString
        {
            get
            {
                string p = "";
                string connection = _conSettings.Host;
                bool pooling = _conSettings.Pooling;
                int minPool = _conSettings.MinPoolSize;
                int maxPool = _conSettings.MaxPoolSize;

                if (pooling)
                    p = "Pooling=true;MinPoolSize=" + minPool + ";MaxPoolSize=" + maxPool + ";";
                else
                    p = "Pooling=false;";

                return string.Concat(connection, p);
            }
        }
    }
}