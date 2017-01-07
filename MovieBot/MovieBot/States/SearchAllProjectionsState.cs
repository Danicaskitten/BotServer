using Microsoft.Bot.Connector;
using MovieBot.Contract;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace MovieBot.States
{
    public class SearchAllProjectionsState : SearchState
    {
        public string ChannelType { get; set; }
        public string UserID { get; set; }
        public string ChoosenMovie { get; set; }
        public List<Cinema> cinemaList { get; set; }
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

        private StateReply stateZero(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                string replayMessage = "Fantastic! Tell me where you would like to go";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            else
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
                    StateNum = 1;
                    string replayMessage = "Please tell me when you want to go to the cinema";
                    StateReply reply = ReplyUtility.generateWeekDayReply(replayMessage);
                    return reply;
                }
                else
                {
                    List<Location> resultList = BingMapsUtility.getLocationFromLocality(userInput);
                    if (resultList == null)
                    {
                        string replyMessage = "What a pity ! I did not find your city in the Bing database. Please, can you give me a bigger city near your location ?";
                        StateReply replay = new StateReply(false, replyMessage);
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
                            string replyMessage = "Please tell me when you want to go to the cinema";
                            StateReply reply = ReplyUtility.generateWeekDayReply(replyMessage);
                            return reply;
                        }
                        else
                        {
                            this.locationList = resultList;
                            string replayMessage = "These are all the cities that match your request";
                            StateReply replay = new StateReply(false, replayMessage, "herocard");
                            string heroCardTitle = "Please select your city";

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

                            replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                            return replay;
                        }
                    }
                }
            }
        }

        private StateReply stateOne(string userInput)
        {
            if (userInput.Contains("selectedday="))
            {
                string selectedDay = userInput.Replace("selectedday=", String.Empty);
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                this.dateChoosen = DateTime.ParseExact(selectedDay, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                string request = "v2/movies/near/"+this.locationFound.Latitude.ToString(nfi) + "/" + this.locationFound.Longitude.ToString(nfi);
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd") + "&maxRange=50";
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                MovieList movieArray = ConnectionUtility.deserialise<MovieList>(response);

                if (movieArray.Data.Count != 0)
                {
                    string replayMessage = "This is the list of all the Movies that are available near your Location";
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "Please select only one movie";

                    List<CardAction> cardButtons = new List<CardAction>();

                    foreach (Movie movie in movieArray.Data)
                    {
                        string title = movie.Title;
                        string value = "movieSelected=" + movie.ImdbID;
                        CardAction plButton = new CardAction()
                        {
                            Value = value,
                            Type = "imBack",
                            Title = title
                        };
                        cardButtons.Add(plButton);
                    }

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select one");
                    StateNum = 2;
                    return replay;
                }
                else
                {
                    string replyMessage = "I did not found any movies available in the day that you have selected. Please try with another day or looking for another cinema";
                    StateReply reply = ReplyUtility.generateWeekDayReply(replyMessage);
                    return reply;
                }
            }
            else
            {
                return null;
            }
        }

        private StateReply stateTwo(string userInput)
        {
            if (userInput.Contains("movieselected="))
            {
                string selectedMovieID = userInput.Replace("movieselected=", String.Empty);
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                this.ChoosenMovie = selectedMovieID;
                string request = "v2/movies/id/" + this.ChoosenMovie + "/cinemas/" + this.locationFound.Latitude.ToString(nfi) + "/" + this.locationFound.Longitude.ToString(nfi);
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd") + "&maxRange=50";
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                CinemaList cinemaArray = ConnectionUtility.deserialise<CinemaList>(response);

                if (cinemaArray.Data.Count != 0)
                {
                    string replayMessage = "This is the list of cinema where your movie is available. Please select your favorite one";
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "Here they are!";

                    List<CardAction> cardButtons = new List<CardAction>();

                    foreach (Cinema cinema in cinemaArray.Data)
                    {
                        string title = cinema.Name;
                        string value = "CinemaSelected=" + cinema.CinemaID;
                        CardAction plButton = new CardAction()
                        {
                            Value = value,
                            Type = "imBack",
                            Title = title
                        };
                        cardButtons.Add(plButton);
                    }

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select one option");
                    StateNum = 3;
                    return replay;
                }
                else
                {
                    string replayMessage = "I did not find any cinema in my database. Please make a new search with a new title";
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
                string request = "v2/projections/list/" + ChoosenMovie + "/" + selectedCinemaID;
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd");
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                ProjectionsList cinemaArray = ConnectionUtility.deserialise<ProjectionsList>(response);

                if (cinemaArray.Data.Count != 0)
                {
                    string replayMessage = "These are all the projections that I have found. If you want to return in the cinema selection select the back option";
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "Here they are!";

                    List<CardAction> cardButtons = new List<CardAction>();

                    foreach (Projection proj in cinemaArray.Data)
                    {
                        string title = "Time Slot: " + proj.Time;
                        string value = "CinemaSelected=" + proj.CinemaID + "MovieSelected=" + proj.ImdbID + "TimeSelected=" + proj.Time + "DateSelected=" + proj.Date;
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

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select your favorite one");
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