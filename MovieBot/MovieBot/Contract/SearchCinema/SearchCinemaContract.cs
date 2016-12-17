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

    public class Movie
    {
        public string Title { get; set; }
        public string ImdbID { get; set; }
        public string Poster { get; set; }
        public string Runtime { get; set; }
        public string Plot { get; set; }
        public string Genre { get; set; }
    }

    public class MovieList
    {
        public List<Movie> Data { get; set; }
    }

    public class CinemaList
    {
        public List<Cinema> Data { get; set; }
    }
}