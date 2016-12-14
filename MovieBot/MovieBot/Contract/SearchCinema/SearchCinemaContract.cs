using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Contract.SearchCinema
{
    public class Cinema
    {
        public string Name { get; set; }
        public int CinemaID { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public object PhoneNumber { get; set; }
        public object Region { get; set; }
        public object Province { get; set; }
        public string City { get; set; }
    }

    public class CinemaList
    {
        public List<Cinema> Data { get; set; }
    }
}