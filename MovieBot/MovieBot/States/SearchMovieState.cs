using Microsoft.Bot.Connector;
using MovieBot.Contract;
using MovieBot.Parser;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace MovieBot.States
{
    /// <summary>
    /// <see cref="SearchState"/> designed for handling the "Search Cinema" request
    /// </summary>
    public class SearchMovieState : SearchState
    {
        public Point locationFound { get; set; }
        public DateTime dateChoosen { get; set; }
        public int StateNum { get; set; }
        public List<Location> locationList { get; set; }

        /// <inheritdoc />
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
                string replayMessage = "Cool! Tell me which film you would like to see";
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
                    this.SelectedMovie = selected_movie;
                    string replayMessage = "Perfect ! I've found that " + this.SelectedMovie.Title + " is now in the cinema. Write me your city and I will provide you all the projections near you";
                    StateReply replay = new StateReply(false, replayMessage);
                    StateNum = 1;
                    return replay;
                }
                else
                {
                    string replayMessage = "I did not find any available movie with that title. Please try with another one.";
                    StateReply replay = new StateReply(false, replayMessage);
                    return replay;
                }
            }
        }

        private StateReply stateOne(string userInput)
        {
            string toBeReplace = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Location, true);
            if (userInput.Contains(toBeReplace))
            {
                string result = userInput.Replace(toBeReplace, String.Empty);
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
                        string replyMessage = "These are all the cities that match your request";
                        StateReply reply = this.generateStateReplyForLocation(locationList, replyMessage);
                        return reply;
                    }
                }
            }
        }

        private StateReply stateTwo(string userInput)
        {
            string toBeReplace = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Day, true);
            if (userInput.Contains(toBeReplace))
            {
                string selectedDay = userInput.Replace(toBeReplace, String.Empty);
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                this.dateChoosen = DateTime.ParseExact(selectedDay, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                string request = "v2/movies/id/" + this.SelectedMovie.ImdbID + "/cinemas/"+this.locationFound.Latitude.ToString(nfi)+"/"+this.locationFound.Longitude.ToString(nfi);
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd") + "&maxRange=100";
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                CinemaList cinemaArray = ConnectionUtility.deserialise<CinemaList>(response);

                if (cinemaArray.Data.Count != 0)
                {
                    string replyMessage = "This is the list of cinema where your movie is available. Please select your favorite one";
                    StateReply reply = this.generateStateReplyForCinema(cinemaArray.Data, replyMessage);
                    StateNum = 3;
                    return reply;
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
            string toBeReplaced = ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Cinema, true);
            if (userInput.Contains(toBeReplaced))
            {
                string selectedCinemaID = userInput.Replace(toBeReplaced, String.Empty);
                Char delimiter = '&';
                String[] substrings = selectedCinemaID.Split(delimiter);
                this.saveCinema(substrings[0], substrings[1]);

                string request = "v2/projections/list/"+ SelectedMovie.ImdbID + "/" + SelectedCinema.CinemaID;
                string requestWithParameter = request + "/?StartDate=" + this.dateChoosen.ToString("yyyy-MM-dd") + "&EndDate=" + this.dateChoosen.AddDays(1).ToString("yyyy-MM-dd");
                string urlRequest = ConnectionUtility.CreateGetRequest(requestWithParameter);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                ProjectionsList projectionList = ConnectionUtility.deserialise<ProjectionsList>(response);
                this.sentProjections = projectionList.Data;

                if (projectionList.Data.Count != 0)
                {
                    StateReply reply = this.generateStateReplyForProjections(projectionList.Data);
                    StateNum = 4;
                    return reply;
                }
                else
                {
                    StateReply replay = this.stateTwo(ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Day, true) + dateChoosen.ToString("MM/dd/yyyy"));
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
            if (userInput.Equals("goback"))
            {
                StateReply replay = this.stateTwo(ReplyUtility.generateValueReplyForHeroCard(ValueEnum.Day, true) + dateChoosen.ToString("MM/dd/yyyy"));
                return replay;
            }
            else
            {
                this.saveProjection(userInput);
                return this.reserveYourSeat();
            }
        }

    }
}