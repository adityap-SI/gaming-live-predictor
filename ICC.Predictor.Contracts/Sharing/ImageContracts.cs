using System;
using System.Collections.Generic;
using System.Text;

namespace ICC.Predictor.Contracts.Sharing
{
    public class Coordinate
    {
        public string entity { get; set; }
        public int xPos { get; set; }
        public int yPos { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Config
    {
        public int pointsXDifference { get; set; }
        public int pointsSubTitleXDifference { get; set; }
        public List<Coordinate> coordinates { get; set; }
        public List<FontDetail> font_details { get; set; }
    }

    public class FontDetail
    {
        public string entity { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }
    }
}
