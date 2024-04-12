using System;
using System.Collections.Generic;

namespace ICC.Predictor.Contracts.Configuration
{
    public class Application
    {
        public Properties Properties { get; set; }
        public Connection Connection { get; set; }
        public SMTP SMTP { get; set; }
        public Cookies Cookies { get; set; }

        public API API { get; set; }
        public Admin Admin { get; set; }

        public CustomSwaggerConfig CustomSwaggerConfig { get; set; }
    }

    #region "Children "

    public class Properties
    {
        public int TourId { get; set; }
        public string ClientName { get; set; }
        public List<string> Languages { get; set; }
        public int OptionCount { get; set; }
    }

    public class Connection
    {
        public string Environment { get; set; }
        public AWSConfig AWS { get; set; }
        public Redis Redis { get; set; }
        public Postgre Postgre { get; set; }
    }

    public class AWSConfig
    {
        public string S3Bucket { get; set; }
        public string S3FolderPath { get; set; }
        public bool Apply { get; set; }
        public bool UseCredentials { get; set; }
        public string IOSARN { get; set; }
        public string AndroidARN { get; set; }
    }

    public class Redis
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool Apply { get; set; }
    }

    public class Postgre
    {
        public string Host { get; set; }
        public string Schema { get; set; }
        public bool Pooling { get; set; }
        public int MinPoolSize { get; set; }
        public int MaxPoolSize { get; set; }
    }

    public class SMTP
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Cookies
    {
        public int ExpiryDays { get; set; }
        public string Domain { get; set; }
    }

    public class CustomSwaggerConfig
    {
        public List<string> BasePathList { get; set; }

        public string SwaggerConfig { get; set; }
    }

    #endregion "Children "
}