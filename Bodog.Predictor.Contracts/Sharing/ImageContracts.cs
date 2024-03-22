using System;
using System.Collections.Generic;
using System.Text;

namespace Bodog.Predictor.Contracts.Sharing
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
        public Int32 pointsXDifference { get; set; }
        public Int32 pointsSubTitleXDifference { get; set; }
        public List<Coordinate> coordinates { get; set; }
        public List<FontDetail> font_details { get; set; }
    }

    public class FontDetail
    {
        public string entity { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public Int32 Red { get; set; }
        public Int32 Green { get; set; }
        public Int32 Blue { get; set; }
        public Int32 Alpha { get; set; }
    }
}
