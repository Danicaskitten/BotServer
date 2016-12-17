using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MovieBot.ReplyManagers;


namespace MovieBot.Parser
{
    /// <summary>
    /// This abstract class is the model for all the parser in this project
    /// </summary>
    public abstract class Parser
    {
        protected Activity activity;
        protected ConnectorClient connector;
        protected ReplyManager replyManager;

        /// <summary>
        /// This is the basic constructor for a Parser
        /// </summary>
        /// <param name="activity">User Activity</param>
        /// <param name="connector">Generated ConnectorClient</param>
        public Parser(Activity activity, ConnectorClient connector)
        {
            this.activity = activity;
            this.connector = connector;
        }
        /// <summary>
        /// This method is used to effectively analyze the incoming message in order to produce the
        /// correct responce.It will return the activity that needs to be sent back to the user.
        /// </summary>
        /// <returns>
        /// This method returns the activity or null if something go wrong in the parsing process
        /// </returns>
        public abstract Task<Activity> computeParsing();
        /// <summary>
        /// Use this mathod in order to know if this parser can process or not a specific user-input
        /// </summary>
        /// <param name="input">Message sent by the user</param>
        /// <returns>Returns True if it can handle the message or Flase on the contrary</returns>
        public abstract Boolean haveAnswer(string input);
    }
}