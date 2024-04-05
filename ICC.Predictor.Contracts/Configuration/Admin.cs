using System;
using System.Collections.Generic;

namespace ICC.Predictor.Contracts.Configuration
{
    public class Admin
    {
        public List<Authorization> Authorization { get; set; }
        public Feed Feed { get; set; }
        public string TemplateUri { get; set; }
    }

    public class Authorization
    {
        public string User { get; set; }
        public string Password { get; set; }
        public List<string> Pages { get; set; }
    }

    public class Feed
    {
        public string API { get; set; }
        public string Client { get; set; }
    }
}