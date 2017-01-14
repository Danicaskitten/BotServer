using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MovieBot.Contract
{
    /// <summary>
    /// Movie Object retrieved from the BackEnd REST APIs
    /// </summary>
    public class Movie
    {
        public string Title { get; set; }
        public string ImdbID { get; set; }
        public object Poster { get; set; }
        public object Runtime { get; set; }
        public object Plot { get; set; }
        public object Genre { get; set; }
    }

    /// <summary>
    /// Cinema Object retrieved from the BackEnd REST APIs
    /// </summary>
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

    /// <summary>
    /// Projection Object retrieved from the BackEnd REST APIs
    /// </summary>
    public class Projection
    {
        public int ProjectionID { get; set; }
        public string ImdbID { get; set; }
        public int CinemaID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int FreeSeats { get; set; }
    }

    /// <summary>
    /// List of <see cref="Projection"/> retrieved from the BackEnd REST APIs
    /// </summary>
    public class ProjectionsList
    {
        public List<Projection> Data { get; set; }
    }

    /// <summary>
    /// List of <see cref="Cinema"/> retrieved from the BackEnd REST APIs
    /// </summary>
    public class CinemaList
    {
        public List<Cinema> Data { get; set; }
    }

    /// <summary>
    /// List of <see cref="Movie"/> retrieved from the BackEnd REST APIs
    /// </summary>
    public class MovieList
    {
        public List<Movie> Data { get; set; }
    }
}