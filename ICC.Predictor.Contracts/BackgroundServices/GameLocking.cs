using System;
using System.Collections.Generic;

namespace ICC.Predictor.Contracts.BackgroundServices
{
    public class LockList
    {
        public List<int> MatchIdList { get; set; }
        public List<int> MatchdayIdList { get; set; }
    }
}
