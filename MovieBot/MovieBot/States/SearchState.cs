using MovieBot.Contract;
using MovieBot.Parser;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBot.States
{
    public abstract class SearchState
    {
        protected Movie selectedMovie;
        protected Cinema selectedCinema;
        protected Projection selectedProjection;
        public abstract StateReply getReplay(string userInput);

        protected void reserveYourSeat()
        {
            //TODO add the link card
        }

        protected void saveMovie(string id, string title)
        {
            Movie newMovie = new Movie
            {
                ImdbID = id,
                Title = title
            };
            this.selectedMovie = newMovie;
        }
        protected void saveCinema(string id, string name)
        {
            Cinema newCinema = new Cinema
            {
                CinemaID = Convert.ToInt32(id),
                Name = name
            };
            this.selectedCinema = newCinema;
        }

        protected void saveProjection(string id, string date, string time)
        {
            Projection newProj = new Projection
            {
                ProjectionID = Convert.ToInt32(id),
                Date = date,
                Time = time
            };
            this.selectedProjection = newProj;
        }
    }
}
