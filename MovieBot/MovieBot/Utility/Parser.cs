using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MovieBot.ReplyManagers;

namespace MovieBot.Utility
{
    public abstract class Parser
    {
        protected Activity activity;
        protected ConnectorClient connector;
        protected ReplyManager replyManager;

        public Parser(Activity activity, ConnectorClient connector)
        {
            this.activity = activity;
            this.connector = connector;
        }

        public abstract Task<Activity> computeParsing();
        public abstract Boolean haveAnswer(string input);
    }
}