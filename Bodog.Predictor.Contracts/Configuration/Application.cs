using System;
using System.Collections.Generic;

namespace Bodog.Predictor.Contracts.Configuration
{
    public class Application
    {
        public Properties Properties { get; set; }
        public Connection Connection { get; set; }
        public SMTP SMTP { get; set; }
        public Cookies Cookies { get; set; }

        public API API { get; set; }
        public Admin Admin { get; set; }
    }

    #region "Children "

    public class Properties
    {
        public Int32 TourId { get; set; }
        public String ClientName { get; set; }
        public List<String> Languages { get; set; }
        public Int32 OptionCount { get; set; }
    }

    public class Connection
    {
        public String Environment { get; set; }
        public AWSConfig AWS { get; set; }
        public Redis Redis { get; set; }
        public Postgre Postgre { get; set; }
    }

    public class AWSConfig
    {
        public String S3Bucket { get; set; }
        public String S3FolderPath { get; set; }
        public bool Apply { get; set; }
        public bool UseCredentials { get; set; }
        public String IOSARN { get; set; }
        public String AndroidARN { get; set; }
    }

    public class Redis
    {
        public String Server { get; set; }
        public Int32 Port { get; set; }
        public bool Apply { get; set; }
    }

    public class Postgre
    {
        public String Host { get; set; }
        public String Schema { get; set; }
        public bool Pooling { get; set; }
        public Int32 MinPoolSize { get; set; }
        public Int32 MaxPoolSize { get; set; }
    }

    public class SMTP
    {
        public String Host { get; set; }
        public Int32 Port { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
    }

    public class Cookies
    {
        public Int32 ExpiryDays { get; set; }
        public String Domain { get; set; }
    }

    #endregion "Children "
}