using Microsoft.Bot.Connector;
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
        public Movie SelectedMovie { get; set; }
        public Cinema SelectedCinema { get; set; }
        public Projection SelectedProjection { get; set; }
        public abstract StateReply getReplay(string userInput);

        protected StateReply reserveYourSeat()
        {
            string url = "http://moviebot-rage.azurewebsites.net/static/reservation/login.html?cinemaName=" + SelectedCinema.Name + "&movieName=" + SelectedMovie.Title
            + "&date=" + SelectedProjection.Date + "&time=" + SelectedProjection.Time + "&freeSeats=" + SelectedProjection.FreeSeats;
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

        protected void saveMovie(string id, string title)
        {
            Movie newMovie = new Movie
            {
                ImdbID = id,
                Title = title
            };
            this.SelectedMovie = newMovie;
        }
        protected void saveCinema(string id, string name)
        {
            Cinema newCinema = new Cinema
            {
                CinemaID = Convert.ToInt32(id),
                Name = name
            };
            this.SelectedCinema = newCinema;
        }

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
