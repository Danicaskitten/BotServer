using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Contract
{
    /// <summary>
    /// Object used to save Coordinates by a <see cref="MovieBot.States.SearchState"/>
    /// </summary>
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }

    /// <summary>
    /// Container for a <see cref="Point"/> Object
    /// </summary>
    public class Location
    {
        public string Name { get; set; }
        public Point Coordinates { get; set; }
    }
}