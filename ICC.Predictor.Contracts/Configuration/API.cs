using System;

namespace ICC.Predictor.Contracts.Configuration
{
    public class API
    {
        public Authentication Authentication { get; set; }
        public string Domain { get; set; }
    }

    public class Authentication
    {
        public string Header { get; set; }
        public string Backdoor { get; set; }
    }
}