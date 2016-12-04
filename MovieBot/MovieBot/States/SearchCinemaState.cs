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
    public class SearchCinemaState : SearchState
    {
        public string ChannelType { get; set; }
        public string UserID { get; set; }
        public Movie ChoosenMovie { get; set; }
        public bool RequestedCinema { get; set; }
        public Projection ChoosenProjection { get; set; }
        public override StateReply getReplay(string userInput)
        {

            if (string.IsNullOrEmpty(userInput))
            {
                string replayMessage = "Cool! Tell me which is your preferite cinema";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            if (!RequestedCinema)
            {
                string request = "v1/Search/CinemaFromName?name=" + userInput;
                string urlRequest = ConnectionUtility.CreateGetRequest(request);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonString = reader.ReadToEnd();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                //Mancano le API
                //Qui devo chiedere se è il cinema giusto
                /*CinemaFromMovieOutputModelList cinemaFromMovieOutputModelArray = Newtonsoft.Json.JsonConvert.DeserializeObject<CinemaFromMovieOutputModelList>(jsonString);
                CinemaFromMovieOutputModel fisrtOnly = cinemaFromMovieOutputModelArray.Data.First();

                string replayMessage = "These are all the projections that I've found. Please click only one choice";
                StateReply replay = new StateReply(false, replayMessage, "herocard");
                string heroCardTitle = "These are the projections available in Cinema " + fisrtOnly.Cinema.Name;

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
                RequestedCinema = true;*/

                return null;
            }
            if (RequestedCinema)
            {
                //Qui prendo il cinema che butta fuori l'utente e chiamo le API che mi restituiscano
                //l'elenco dei film in proiezione
                return null;
            }
            if (ChoosenMovie == null)
            {
                //Qui mi devo salvare il film che mi farò mandare in ChoosenMovie e poi chiedere per tutte
                //le proiezioni
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
            if (ChoosenProjection == null)
            {
                string replayMessage = "Your reservation has successfully been completed. Enjoy your Movie !!";
                StateReply replay = new StateReply(true, replayMessage);
                return replay;
            }
            return null;
        }
    }
}