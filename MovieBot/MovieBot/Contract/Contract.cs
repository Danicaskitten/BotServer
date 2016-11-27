using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MovieBot.Contract
{
    public class Movie
    {
        public string Title { get; set; }
        public string ImdbDb { get; set; }
        public object Poster { get; set; }
        public object Runtime { get; set; }
        public object Plot { get; set; }
        public object Genre { get; set; }
    }

    public class Cinema
    {
        public string Name { get; set; }
        public string CinemaID { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public object PhoneNumber { get; set; }
        public object Region { get; set; }
        public object Province { get; set; }
        public string City { get; set; }
    }

    public class Projection
    {
        public string ImdbID { get; set; }
        public string CinemaID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

    public class CinemaFromMovieOutputModel
    {
        public Cinema Cinema { get; set; }
        public List<Projection> Projections { get; set; }
        public string ImdbId { get; set; }
    }
    public class CinemaFromMovieOutputModelList
    {
        public List<CinemaFromMovieOutputModel> Data { get; set; }
    }

    public class MovieList
    {
        public List<Movie> Data { get; set; }
    }
}