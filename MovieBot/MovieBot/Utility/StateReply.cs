using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Utility
{
    public class StateReply
    {
        private bool _finalState;
        private string _replayMessage;
        private string _special;
        private HeroCard _heroCard;
        private CardAction _card;

        public StateReply(bool isFinalState, string replayMessage)
        {
            _finalState = isFinalState;
            _replayMessage = replayMessage;
        }

        public StateReply(bool isFinalState, string replayMessage, string special)
        {
            _finalState = isFinalState;
            _replayMessage = replayMessage;
            _special = special;
        }

        public string GetReplayMessage
        {
            get { return _replayMessage; }
        }
        public bool IsFinalState
        {
            get { return _finalState; }
        }
        public HeroCard HeroCard
        {
            get { return _heroCard; }
            set { _heroCard = value; }
        }
        public CardAction Card
        {
            get { return _card; }
            set { _card = value; }
        }
        public string GetSpecial
        {
            get { return _special; }
        }
    }
}