using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Contract
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }

    public class Location
    {
        public string Name { get; set; }
        public Point Coordinates { get; set; }
    }
}