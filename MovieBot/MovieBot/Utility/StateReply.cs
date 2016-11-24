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

        public StateReply(bool isFinalState, string replayMessage)
        {
            _finalState = isFinalState;
            _replayMessage = replayMessage;
        }

        public string GetReplayMessage
        {
            get { return _replayMessage; }
        }
        public bool IsFinalState
        {
            get { return _finalState; }
        }
    }
}