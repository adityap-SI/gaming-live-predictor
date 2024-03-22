using System;
using System.Collections.Generic;

namespace Bodog.Predictor.Contracts.Configuration
{
    public class Admin
    {
        public List<Authorization> Authorization { get; set; }
        public Feed Feed { get; set; }
        public String TemplateUri { get; set; }
    }

    public class Authorization
    {
        public String User { get; set; }
        public String Password { get; set; }
        public List<String> Pages { get; set; }
    }

    public class Feed
    {
        public String API { get; set; }
        public String Client { get; set; }
    }
}