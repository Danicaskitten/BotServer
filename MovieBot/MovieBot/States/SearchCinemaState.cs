using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.States
{
    public class SearchCinemaState
    {
        private string _channelType;
        private int _userID;
        private string _choosenTitle;
        private string _choosenCinema;

        public string TitleProperty
        {
            set { _choosenTitle = value; }
        }
        public string CinemaProperty
        {
            set { _choosenCinema = value; }
        }
        public int UserIDProperty
        {
            get { return _userID; }
        }
        public int ChannelTypeProperty
        {
            get { return _userID; }
        }

        public SearchCinemaState(string channelType, int userID)
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
            if (_choosenTitle == null)
            {
                //TODO sistemare le connessioni
            }
            return null;
        }
    }
}