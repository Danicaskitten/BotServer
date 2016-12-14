using Microsoft.Bot.Connector;
using MovieBot.Contract;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Point locationFound { get; set; }
        public DateTime dateChoosen { get; set; }
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
                case 4:
                    StateReply reply4 = stateFour(userInput);
                    return reply4;
                default:
                    return null;
            }
        }

        private StateReply stateOne(string userInput)
        {
            if (userInput.Contains("selectedlocation="))
            {
                string result = userInput.Replace("selectedlocation=", String.Empty);
                foreach (Location item in locationList)
                {
                    if (item.Name.ToLower() == result)
                    {
                        this.locationFound = new Point
                        {
                            Latitude = item.Coordinates.Latitude,
                            Longitude = item.Coordinates.Longitude
                        };
                    }
                }
                StateNum = 2;
                StateReply reply = ReplyUtility.generateWeekDayReply();
                return reply;
            }
            else
            {
                List<Location> resultList = BingMapsUtility.getLocationFromLocality(userInput);
                if (resultList == null)
                {
                    string replayMessage = "I didin't found your city in the Bing database. Please, can you give me a bigger city near to your location ?";
                    StateReply replay = new StateReply(false, replayMessage);
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
                        StateReply reply = ReplyUtility.generateWeekDayReply();
                        return reply;
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

                        replay.HeroCard = replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                        return replay;
                    }
                }
            }
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
                string request = "v2/movies/title/" + userInput +"/";
                string urlRequest = ConnectionUtility.CreateGetRequest(request);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                MovieList movieArray = ConnectionUtility.deserialise<MovieList>(response);

                if (movieArray.Data.Count != 0)
                {
                    Movie selected_movie = movieArray.Data.First();
                    this.ChoosenMovie = selected_movie;
                    string replayMessage = "Yai ! I've found that " + this.ChoosenMovie.Title + " is now in the cinemas. Write me your city  and I will provide you all the near projections ";
                    StateReply replay = new StateReply(false, replayMessage);
                    StateNum = 1;
                    return replay;
                }
                else
                {
                    string replayMessage = "I didin't found any available movies with that title. Please try with another one.";
                    StateReply replay = new StateReply(false, replayMessage);
                    return replay;
                }
            }
        }

        private StateReply stateTwo(string userInput)
        {
            if (userInput.Contains("selectedday="))
            {
                string selectedDay = userInput.Replace("selectedday=", String.Empty);
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                this.dateChoosen = DateTime.ParseExact(selectedDay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                string request = "v2/movies/id/" + this.ChoosenMovie.ImdbID + "/cinemas/"+this.locationFound.Latitude.ToString(nfi)+"/"+this.locationFound.Longitude.ToString(nfi);
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd") + "&maxRange=100";
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                CinemaList cinemaArray = ConnectionUtility.deserialise<CinemaList>(response);

                if (cinemaArray.Data.Count != 0)
                {
                    string replayMessage = "These are all the cinemas where your movie is available. Please select only one choice";
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "These are all the cinemas";

                    List<CardAction> cardButtons = new List<CardAction>();

                    foreach (Cinema cinema in cinemaArray.Data)
                    {
                        string title = cinema.Name;
                        string value = "CinemaSelected="+cinema.CinemaID;
                        CardAction plButton = new CardAction()
                        {
                            Value = value,
                            Type = "imBack",
                            Title = title
                        };
                        cardButtons.Add(plButton);
                    }

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                    StateNum = 3;
                    return replay;
                }
                else
                {
                    string replayMessage = "I didin't found any cinemas in my database. Please restart againg the search movie with a new title";
                    StateReply replay = new StateReply(true, replayMessage);
                    return replay;
                }
            }
            else
            {
                return null;
            }
        }

        private StateReply stateThree(string userInput)
        {
            if (userInput.Contains("cinemaselected="))
            {
                string selectedCinemaID = userInput.Replace("cinemaselected=", String.Empty);
                string request = "v2/projections/list/"+selectedCinemaID+"/" + this.ChoosenMovie.ImdbID;
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd");
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                ProjectionsList cinemaArray = ConnectionUtility.deserialise<ProjectionsList>(response);

                if (cinemaArray.Data.Count != 0)
                {
                    string replayMessage = "These are all the projections that I've found. If you wanna go back in the cinema selection press back button";
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "These are all the projections";

                    List<CardAction> cardButtons = new List<CardAction>();

                    foreach (Projection proj in cinemaArray.Data)
                    {
                        string title = "Time Slot: "+ proj.Time;
                        string value = "CinemaSelected="+proj.CinemaID+"MovieSelected="+proj.ImdbID+"TimeSelected="+proj.Time+"DateSelected="+proj.Date;
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
                        Value = "Back",
                        Type = "imBack",
                        Title = "Back"
                    };
                    cardButtons.Add(plButton1);

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                    StateNum = 4;
                    return replay;
                }
                else
                {
                    StateReply replay = this.stateTwo("selectedDay=" + dateChoosen.ToString("MM/dd/yyyy"));
                    return replay;
                }
            }
            else
            {
                return null;
            }
        }
        private StateReply stateFour(string userInput)
        {
            if (userInput.Equals("back"))
            {
                StateReply replay = this.stateTwo("selectedday=" + dateChoosen.ToString("MM/dd/yyyy"));
                return replay;
            }
            else
            {
                string replayMessage = "Your reservation has successfully been completed. Enjoy your Movie !!";
                StateReply replay = new StateReply(true, replayMessage);
                return replay;
            }
        }

    }
}