using Microsoft.Bot.Connector;
using MovieBot.Contract;
using MovieBot.Parser;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MovieBot.States
{
    /// <summary>
    /// This class is the core of the Bot, it manages user requests, save all the important data and retrieves information from the APIs
    /// </summary>
    public abstract class SearchState
    {
        /// <summary>
        /// <see cref="Movie"/> selected by the User
        /// </summary>
        public Movie SelectedMovie { get; set; }
        /// <summary>
        /// <see cref="Cinema"/> selected by the User
        /// </summary>
        public Cinema SelectedCinema { get; set; }
        /// <summary>
        /// <see cref="Projection"/> selected by the User
        /// </summary>
        public Projection SelectedProjection { get; set; }
        
        /// <summary>
        /// This method manages the UserInput and returns the rigth <see cref="StateReply"/>
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public abstract StateReply getReplay(string userInput);

        /// <summary>
        /// This method manages the Reservation state and returns the right <see cref="StateReply"/>
        /// </summary>
        /// <returns></returns>
        protected StateReply reserveYourSeat()
        {
            string url = "https://moviebot-rage.azurewebsites.net/static/reservation/login.html?cinemaName=" + HttpUtility.UrlPathEncode(SelectedCinema.Name) + "&movieName=" + HttpUtility.UrlPathEncode(SelectedMovie.Title)+ "&date=" + HttpUtility.UrlPathEncode(SelectedProjection.Date) + "&time=" + HttpUtility.UrlPathEncode(SelectedProjection.Time) + "&freeSeats=" + SelectedProjection.FreeSeats;
            CardAction plButton = new CardAction()
            {
                Value = url,
                Type = "openUrl",
                Title = "Reserve Your Seat"
            };
            List<CardAction> cardButtons = new List<CardAction>();
            cardButtons.Add(plButton);
            string heroCardTitle = "Reserve Your Seat";
            StateReply replyToSend = new StateReply(true, "Click the button to complete the Reservation", "herocard");

            replyToSend.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please complete your reservation process");
            return replyToSend;
        }

        /// <summary>
        /// This method create e save the Movie with a given Id and Title
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        protected void saveMovie(string id, string title)
        {
            Movie newMovie = new Movie
            {
                ImdbID = id,
                Title = title
            };
            this.SelectedMovie = newMovie;
        }

        /// <summary>
        /// This method create e save the Cinema with a given Id and Name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        protected void saveCinema(string id, string name)
        {
            Cinema newCinema = new Cinema
            {
                CinemaID = Convert.ToInt32(id),
                Name = name
            };
            this.SelectedCinema = newCinema;
        }

        /// <summary>
        /// This method create e save the Projection parsing the given userInput
        /// </summary>
        /// <param name="userInput"></param>
        protected void saveProjection(string userInput)
        {
            string selectedProj = userInput.Replace("timeselected=", String.Empty);
            selectedProj = selectedProj.Replace("dateselected=", "&");
            selectedProj = selectedProj.Replace("projselected=", "&");
            selectedProj = selectedProj.Replace("freeseats=", "&");
            Char delimiter = '&';
            String[] substrings = selectedProj.Split(delimiter);
            Projection newProj = new Projection
            {
                ProjectionID = Convert.ToInt32(substrings[2]),
                Date = substrings[1],
                FreeSeats = Convert.ToInt32(substrings[3]),
                Time =substrings[0]
            };
            this.SelectedProjection = newProj;
        }
    }
}
