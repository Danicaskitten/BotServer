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
    public class SearchCinemaState : SearchState
    {
        public string ChannelType { get; set; }
        public string UserID { get; set; }
        public Cinema ChoosenCinema { get; set; }
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
                default:
                    return null;
            }
        }

        private StateReply stateZero(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                string replayMessage = "Cool! Tell me in which cinema do you like to go";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            else
            {
                if (!userInput.Contains("selectedcinema=")) {
                    string request = "v2/cinemas/name/" + userInput + "/";
                    string urlRequest = ConnectionUtility.CreateGetRequest(request);
                    WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                    CinemaList cinemaArray = ConnectionUtility.deserialise<CinemaList>(response);
                    this.cinemaList = cinemaArray.Data;

                    if (cinemaArray.Data.Count != 0)
                    {
                        if (cinemaArray.Data.Count > 1)
                        {
                            string replayMessage = "These are all the cinemas that I've found. Please your desired one";
                            StateReply replay = new StateReply(false, replayMessage, "herocard");
                            string heroCardTitle = "Select your cinema";

                            List<CardAction> cardButtons = new List<CardAction>();

                            foreach (Cinema Cinema in cinemaArray.Data)
                            {
                                string title = Cinema.Name + " City: " + Cinema.City;
                                string value = "selectedCinema=" + Cinema.CinemaID;
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
                        else
                        {
                            Cinema selectedCinema = cinemaArray.Data.First();
                            this.ChoosenCinema = selectedCinema;
                            string replyMessage = "Yai ! I've found " + this.ChoosenCinema.Name + ". When do you want to go?";
                            StateReply reply = ReplyUtility.generateWeekDayReply(replyMessage);
                            StateNum = 1;
                            return reply;
                        }
                    }
                    else
                    {
                        string replayMessage = "I didin't found any cinemas matches your input. Please try with another one.";
                        StateReply replay = new StateReply(false, replayMessage);
                        return replay;
                    }
                }
                else
                {
                    string result = userInput.Replace("selectedcinema=", String.Empty);
                    foreach (Cinema cinema in cinemaList)
                    {
                        if (cinema.CinemaID == Int32.Parse(result))
                        {
                            this.ChoosenCinema = cinema;
                        }
                    }

                    string replyMessage = "Perfect, thank you for your help. When do you want to go?";
                    StateReply reply = ReplyUtility.generateWeekDayReply(replyMessage);
                    StateNum = 1;
                    return reply;
                }
            }
        }

        private StateReply stateOne(string userInput)
        {
            if (userInput.Contains("selectedday="))
            {
                string selectedDay = userInput.Replace("selectedday=", String.Empty);
                NumberFormatInfo nfi = new NumberFormatInfo();
                this.dateChoosen = DateTime.ParseExact(selectedDay, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                string request = "v2/cinemas/id/" + this.ChoosenCinema.CinemaID + "/movies";
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd");
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                MovieList movieArray = ConnectionUtility.deserialise<MovieList>(response);

                if (movieArray.Data.Count != 0)
                {
                    string replayMessage = "These are all the Movies that are available in "+ ChoosenCinema.Name;
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "Here they are";

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

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                    StateNum = 2;
                    return replay;
                }
                else
                {
                    string replayMessage = "I didin't found any movies available for the day you selected. Please restart againg the search cinema with a new cinema";
                    StateReply replay = new StateReply(true, replayMessage);
                    return replay;
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
                string request = "v2/projections/list/" + ChoosenCinema.CinemaID + "/" + selectedMovieID;
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd");
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                ProjectionsList cinemaArray = ConnectionUtility.deserialise<ProjectionsList>(response);

                if (cinemaArray.Data.Count != 0)
                {
                    string replayMessage = "These are all the projections that I've found. If you wanna go back in the movie selection press back button";
                    StateReply replay = new StateReply(false, replayMessage, "herocard");
                    string heroCardTitle = "These are all the projections";

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

                    replay.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
                    StateNum = 3;
                    return replay;
                }
                else
                {
                    StateReply replay = this.stateOne("selectedday=" + dateChoosen.ToString("MM/dd/yyyy"));
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
            if (userInput.Equals("back"))
            {
                StateReply replay = this.stateOne("selectedday=" + dateChoosen.ToString("MM/dd/yyyy"));
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