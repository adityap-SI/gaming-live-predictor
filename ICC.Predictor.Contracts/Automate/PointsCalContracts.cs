using System;
using System.Collections.Generic;
using System.Text;

namespace ICC.Predictor.Contracts.Automate
{
    public class Matchdays
    {
        public int GamedayId { get; set; }
        public int PhaseId { get; set; }
        public int Matchday { get; set; }
        public List<int> TeamIds { get; set; }
    }
}
