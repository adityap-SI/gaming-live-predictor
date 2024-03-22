using System;
using System.Collections.Generic;
using System.Text;

namespace Bodog.Predictor.Contracts.Automate
{
    public class Matchdays
    {
        public Int32 GamedayId { get; set; }
        public Int32 PhaseId { get; set; }
        public Int32 Matchday { get; set; }
        public List<Int32> TeamIds { get; set; }
    }
}
