using Microsoft.Bot.Connector;
using MovieBot.Contract;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace MovieBot.States
{
    public class SearchCinemaState
    {
        public string ChannelType { get; set; }
        public string UserID { get; set; }
        public Movie ChoosenMovie { get; set; }
        public bool ChoosenCinema { get; set; }

        public StateReply getReplay(string userInput)
        {
            if(string.IsNullOrEmpty(userInput))
            {
                string replayMessage = "Cool! Tell me which film do you like to see";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            if (ChoosenMovie == null)
            {
                string request = "v1/Search/Movie?title=" + userInput;
                string urlRequest = ConnectionUtility.CreateGetRequest(request);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonString = reader.ReadToEnd();
                Console.WriteLine(jsonString);
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                MovieList movieArray = Newtonsoft.Json.JsonConvert.DeserializeObject<MovieList>(jsonString);
                Movie selected_movie = movieArray.Data.First();
                ChoosenMovie = selected_movie;
                string replayMessage = "Yai ! I've found that " + ChoosenMovie.Title + " is now in the cinemas. Write me your city  and I will provide you all the near projections ";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            if(!ChoosenCinema)
            {
                string request = "v1/Search/CinemaFromMovie?imdbid=" + ChoosenMovie.ImdbDb;
                string urlRequest = ConnectionUtility.CreateGetRequest(request);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonString = reader.ReadToEnd();
                Console.WriteLine(jsonString);
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                CinemaFromMovieOutputModelList cinemaFromMovieOutputModelArray = Newtonsoft.Json.JsonConvert.DeserializeObject<CinemaFromMovieOutputModelList>(jsonString);
                CinemaFromMovieOutputModel fisrtOnly = cinemaFromMovieOutputModelArray.Data.First();

                string replayMessage = "These are all the projections that I've found. Please click only one choice";
                StateReply replay = new StateReply(false, replayMessage,"herocard");
                string heroCardTitle = "These are the projections of " + ChoosenMovie.Title + " that are available in Cinema " + fisrtOnly.Cinema.Name;

                List<CardAction> cardButtons = new List<CardAction>();

                foreach (Projection proj in fisrtOnly.Projections)
                {
                    string title = "Time Slot: " + proj.Time;
                    string value = "Title=" + ChoosenMovie.Title + ";Cinema=" + fisrtOnly.Cinema.Name + ";Date=" + proj.Date + ";Time=" + proj.Time;
                    CardAction plButton = new CardAction()
                    {
                        Value = value,
                        Type = "imBack",
                        Title = title
                    };
                    cardButtons.Add(plButton);
                }

                HeroCard plCard = new HeroCard()
                {
                    Title = heroCardTitle,
                    Subtitle = "Please choose you preferite one",
                    Buttons = cardButtons
                };

                replay.HeroCard = plCard;
                ChoosenCinema = true;

                return replay;
            }
            if (ChoosenCinema)
            {
                string replayMessage = "Your reservation has successfully been completed. Enjoy your Movie !!";
                StateReply replay = new StateReply(true, replayMessage);
                return replay;
            }
            return null;
        }
    }
}