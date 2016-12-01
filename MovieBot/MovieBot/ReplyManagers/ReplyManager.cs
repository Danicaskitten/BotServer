using Microsoft.Bot.Connector;
using MovieBot.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieBot.ReplyManagers
{
    public abstract class ReplyManager
    {
        protected Activity activity;
        protected string input;

        public ReplyManager(Activity activity, string input)
        {
            this.activity = activity;
            this.input = input;
        }

        public abstract Task<Activity> getResponse();
        public abstract Task<Activity> getResponseWithState(SearchMovieState state);

    }
}