using MovieBot.Parser;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBot.States
{
    public abstract class SearchState
    {
        public abstract StateReply getReplay(string userInput);
    }
}
