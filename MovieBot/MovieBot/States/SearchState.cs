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
        /// Channel used by the user, this property can be used in order to differentiate the replies based on the channelType 
        /// </summary>
        public string ChannelType { get; set; }
        /// <summary>
        /// User ID in the channel 
        /// </summary>
        public string UserID { get; set; }
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
        /// List of <see cref="Projection"/> sent to the User
        /// </summary>
        public List<Projection> sentProjections { get; set; }

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
            string url = "https://moviebot-rage.azurewebsites.net/static/reservation/login.html?projID=" + SelectedProjection.ProjectionID;
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
        /// This method create e save the Movie with a given Title and id
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        protected void saveMovie(string title, string id)
        {
            Movie newMovie = new Movie
            {
                ImdbID = id,
                Title = title
            };
            this.SelectedMovie = newMovie;
        }

        /// <summary>
        /// This method create e save the Cinema with a given and Name and id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        protected void saveCinema(string name, string id)
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
            string toBeReplaced = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Projections, true);
            string selectedProj = userInput.Replace(toBeReplaced, String.Empty);
            this.SelectedProjection = this.sentProjections[Convert.ToInt32(selectedProj)];
        }

        /// <summary>
        /// This method receive as parameter the list of <see cref="Projection"/> and return the corresponding <see cref="StateReply"/>
        /// </summary>
        /// <param name="projectionList"></param>
        /// <returns></returns>
        protected StateReply generateStateReplyForProjections(List<Projection> projectionList)
        {
            string replayMessage = "These are all the projections that I have found. If you want to return in the cinema selection select the back option";
            StateReply reply = new StateReply(false, replayMessage, "herocard");
            string heroCardTitle = "Here they are!";

            List<CardAction> cardButtons = new List<CardAction>();

            for (int index = 0; index < projectionList.Count; index++)
            {
                Projection proj = projectionList[index];
                string title = "Time Slot: " + proj.Time + " Free Seats: " + proj.FreeSeats;
                string value = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Projections, false) + index;
                CardAction plButton = new CardAction()
                {
                    Value = value,
                    Type = "imBack",
                    Title = title
                };
                cardButtons.Add(plButton);
            }

            CardAction plButton1 = new CardAction()
            {
                Value = "GoBack",
                Type = "imBack",
                Title = "Back"
            };
            cardButtons.Add(plButton1);

            reply.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select your favorite one");
            return reply;
        }

        /// <summary>
        /// This method receive as parameter the list of <see cref="Movie"/> and return the corresponding <see cref="StateReply"/>
        /// </summary>
        /// <param name="movieList"></param>
        /// <param name="messageReply"></param>
        /// <returns></returns>
        protected StateReply generateStateReplyForMovies(List<Movie> movieList, string messageReply)
        {
            StateReply reply = new StateReply(false, messageReply, "herocard");
            string heroCardTitle = "Please select only one movie";

            List<CardAction> cardButtons = new List<CardAction>();

            foreach (Movie movie in movieList)
            {
                string title = movie.Title;
                string value = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Movie, false) + movie.Title +",ID=" + movie.ImdbID;
                CardAction plButton = new CardAction()
                {
                    Value = value,
                    Type = "imBack",
                    Title = title
                };
                cardButtons.Add(plButton);
            }

            reply.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select one");
            return reply;
        }

        /// <summary>
        /// This method receive as parameter the list of <see cref="Location"/> and return the corresponding <see cref="StateReply"/>
        /// </summary>
        /// <param name="locationList"></param>
        /// <param name="messageReply"></param>
        /// <returns></returns>
        protected StateReply generateStateReplyForLocation(List<Location> locationList, string messageReply)
        {
            StateReply reply = new StateReply(false, messageReply, "herocard");
            string heroCardTitle = "Please select your city";

            List<CardAction> cardButtons = new List<CardAction>();

            foreach (Location item in locationList)
            {
                string title = item.Name;
                string value = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Location, false) + item.Name;
                CardAction plButton = new CardAction()
                {
                    Value = value,
                    Type = "imBack",
                    Title = title
                };
                cardButtons.Add(plButton);
            }

            reply.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
            return reply;
        }

        /// <summary>
        /// This method receive as parameter the list of <see cref="Cinema"/> and return the corresponding <see cref="StateReply"/>
        /// </summary>
        /// <param name="cinemaList"></param>
        /// <param name="messageReply"></param>
        /// <returns></returns>
        protected StateReply generateStateReplyForCinema(List<Cinema> cinemaList, string messageReply)
        {
            StateReply reply = new StateReply(false, messageReply, "herocard");
            string heroCardTitle = "Here they are!";

            List<CardAction> cardButtons = new List<CardAction>();

            foreach (Cinema cinema in cinemaList)
            {
                string title = cinema.Name;
                string value = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Cinema,false) + cinema.Name + ",ID=" + cinema.CinemaID;
                CardAction plButton = new CardAction()
                {
                    Value = value,
                    Type = "imBack",
                    Title = title
                };
                cardButtons.Add(plButton);
            }

            reply.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select one option");
            return reply;
        }
    }
}
