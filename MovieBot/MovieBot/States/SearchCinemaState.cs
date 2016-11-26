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
        private string _channelType;
        private string _userID;
        private Movie _choosenMovie;
        private string _choosenCinema;

        public Movie TitleProperty
        {
            set { _choosenMovie = value; }
        }
        public string CinemaProperty
        {
            set { _choosenCinema = value; }
        }
        public string UserIDProperty
        {
            get { return _userID; }
        }
        public string ChannelTypeProperty
        {
            get { return _userID; }
        }

        public SearchCinemaState(string channelType, string userID)
        {
            _channelType = channelType;
            _userID = userID;
        }

        public StateReply getReplay(string userInput)
        {
            if(string.IsNullOrEmpty(userInput))
            {
                string replayMessage = "Cool! Tell me which film do you like to see";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            if (_choosenMovie == null)
            {
                string request = "Search/Movie?title=" + userInput;
                string urlRequest = ConnectionUtility.CreateGetRequest(request);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonString = reader.ReadToEnd();
                Console.WriteLine(jsonString);
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                Movie[] movieArray = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie[]>(jsonString);
                Movie selected_movie = movieArray[0];
                _choosenMovie = selected_movie;
                string replayMessage = "Yai ! I've find that " + _choosenMovie.Title + " is now in the cinemas. Write me your city  and I will provide you all the near projections ";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            if(_choosenCinema == null)
            {
                string request = "Search/CinemaFromMovie?imdbid=" + _choosenMovie.ImdbDb;
                string urlRequest = ConnectionUtility.CreateGetRequest(request);
                WebResponse response = ConnectionUtility.MakeRequest(urlRequest);
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonString = reader.ReadToEnd();
                Console.WriteLine(jsonString);
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                CinemaFromMovieOutputModel[] cinemaFromMovieOutputModelArray = Newtonsoft.Json.JsonConvert.DeserializeObject<CinemaFromMovieOutputModel[]>(jsonString);
                CinemaFromMovieOutputModel fisrtOnly = cinemaFromMovieOutputModelArray[0];
                //https://blogs.msdn.microsoft.com/tsmatsuz/2016/08/31/microsoft-bot-framework-messages-howto-image-html-card-button-etc/
                string replayMessage = "Yai ! I've find that " + _choosenMovie.Title + " is now in the cinemas. Write me your city  and I will provide you all the near projections ";
                StateReply replay = new StateReply(false, replayMessage);
                return replay;
            }
            return null;
        }
    }
}