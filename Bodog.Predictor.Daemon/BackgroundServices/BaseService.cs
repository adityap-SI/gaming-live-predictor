using System;
using Bodog.Predictor.Interfaces.AWS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bodog.Predictor.Contracts.Configuration;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Interfaces.Asset;

namespace Bodog.Predictor.Daemon.BackgroundServices
{
    public class BaseService<T>
    {
        protected readonly ILogger<T> _Logger;
        protected readonly IOptions<Application> _AppSettings;
        protected readonly IOptions<Contracts.Configuration.Daemon> _ServiceSettings;
        protected readonly IAWS _AWS;
        protected readonly IPostgre _Postgre;
        protected readonly IRedis _Redis;
        protected readonly IAsset _Asset;
        protected readonly Int32 _TourId;
        protected readonly String _Environment;
        protected readonly String _Service;

        public BaseService(ILogger<T> logger, IOptions<Application> appSettings, IOptions<Contracts.Configuration.Daemon> serviceSettings, IAWS aws,
            IPostgre postgre, IRedis redis, IAsset asset)
        {
            _Logger = logger;
            _AppSettings = appSettings;
            _ServiceSettings = serviceSettings;
            _AWS = aws;
            _Postgre = postgre;
            _Redis = redis;
            _Asset = asset;
            _TourId = appSettings.Value.Properties.TourId;
            _Environment = appSettings.Value.Connection.Environment;
            _Service = typeof(T).Name;
        }

        /// <summary>
        /// Catches error and information messages
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="level">Type of log</param>
        /// <param name="ex">Exception object</param>
        public void Catcher(String message, LogLevel level = LogLevel.Information, Exception ex = null)
        {
            try
            {
                String text = $"{_Service} Daemon: {message}";

                if (level == LogLevel.Error)
                {
                    _Logger.LogError(ex, text);
                    Notify($"{_Service} Error", $"{message}<br/>Exception: {ex.Message}<br/>InnerException: {ex.InnerException}");
                }
                else
                    _Logger.LogInformation(text);
            }
            catch { }
        }

        /// <summary>
        /// Send email notification to the recipients address listed in appsettings
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Body content</param>
        public void Notify(string subject, string body)
        {
            try
            {
                String sender = _ServiceSettings.Value.Notification.Sender;
                String recipient = _ServiceSettings.Value.Notification.Recipient;

                _AWS.SendSESMail(sender, recipient, "", "", $"Bodog [{_Environment.ToUpper()}] | {subject}", body, true);
            }
            catch { }
        }
    }
}
