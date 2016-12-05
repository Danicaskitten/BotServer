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
    public class SearchMovieState : SearchState
    {
        public string ChannelType { get; set; }
        public string UserID { get; set; }
        public Movie ChoosenMovie { get; set; }
        public bool ChoosenCinema { get; set; }
        public bool LocationRequested { get; set; }
        public Point locationFound { get; set; }
        public DateTime timeChoosen { get; set; }
        public int StateNum { get; set; }
        public List<Location> locationList { get; set; }

        public override StateReply getReplay(string userInput)
        {
            switch (StateNum)
            {
                case 0:
                    StateReply reply = stateZero(userInput);
                    return reply;
                case 1:
                    StateReply reply1 = stateOne(userInput);
                    return reply1;
                case 2:
                    StateReply reply2 = stateTwo(userInput);
                    return reply2;
                case 3:
                    StateReply reply3 = stateThree(userInput);
                    return reply3;
                default:
                    return null;
            }
        }

        private StateReply stateOne(string userInput)
        {
            if (userInput.Contains("selectedLocation="))
            {
                string result = userInput.Replace("selectedLocation=", String.Empty);
                foreach (Location item in locationList)
                {
                    if (item.Name == result)
                    {
                        this.locationFound = new Point
                        {
                            Latitude = item.Coordinates.Latitude,
                            Longitude = item.Coordinates.Longitude
                        };
                    }
                }
                StateNum += 1;
                //TODO fare il giorno della settimana
            }
            else
            {
                List<Location> resultList = BingMapsUtility.getLocationFromLocality(userInput);
                if (resultList == null)
                {
                    string replayMessage = "I didin't found your city in the Bing database. Please, can you give me a bigger city near to your location ?";
                    StateReply replay = new StateReply(true, replayMessage);
                    return replay;
                }
                else
                {
                    if (resultList.Count == 1)
                    {
                        Location element = resultList.First();
                        this.locationFound = new Point
                        {
                            Latitude = element.Coordinates.Latitude,
                            Longitude = element.Coordinates.Longitude
                        };
                        StateNum += 1;
                        //TODO fare il giorno della settimana
                    }
                    else
                    {
                        this.locationList = resultList;
                        string replayMessage = "These are all the city that I've found. Please click only one choice";
                        StateReply replay = new StateReply(false, replayMessage, "herocard");
                        string heroCardTitle = "Select your city";

                        List<CardAction> cardButtons = new List<CardAction>();

                        foreach (Location item in locationList)
                        {
                            string title = item.Name;
                            string value = "selectedLocation=" + item.Name;
                            CardAction plButton = new CardAction()
                            {
                                Value = value,
                                Type = "imBack",
                                Title = title
                            };
                            cardButtons.Add(plButton);
                        }

                        replay.HeroCard = replay.HeroCard = ReplyUtilitycs.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                        return replay;
                    }
                }
            }
            return null;
        }

        private StateReply stateZero(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                string replayMessage = "Cool! Tell me which film do you like to see";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            else
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
                StateNum += 1;
                return replay;
            }
        }

        private StateReply stateTwo(string userInput)
        {
            //TODO salvare la data e fare la Query Giusta
            string request = "v1/Search/CinemaFromMovie?imdbid=" + ChoosenMovie.ImdbDb;
            string urlRequest = ConnectionUtility.CreateGetRequest(request);
            WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            string jsonString = reader.ReadToEnd();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            CinemaFromMovieOutputModelList cinemaFromMovieOutputModelArray = Newtonsoft.Json.JsonConvert.DeserializeObject<CinemaFromMovieOutputModelList>(jsonString);
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

            replay.HeroCard = ReplyUtilitycs.generateHeroCardStateReply(cardButtons,heroCardTitle,"please select one");
            ChoosenCinema = true;
            StateNum += 1;
            return replay;
        }

        private StateReply stateThree(string userInput)
        {
            string replayMessage = "Your reservation has successfully been completed. Enjoy your Movie !!";
            StateReply replay = new StateReply(true, replayMessage);
            return replay;
        }


    }
}