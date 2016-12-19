using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using MovieBot.States;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace MovieBot.Parser
{
    public class MessageStateParser
    {
        private Activity activity;
        private ConnectorClient connector;
        private ReplyManager replyManager;
        private Activity reply;

        public MessageStateParser(Activity activity, ConnectorClient connector){
            this.activity = activity;
            this.connector = connector;
        }

        /// <summary>
        /// This method is used to return the activity that needs to be sent back to the user.
        /// </summary>
        /// <returns>
        /// This method returns the activity or null if something go wrong in the parsing process
        /// </returns>
        public Activity computeParsing()
        {
            return reply;
        }

        /// <summary>
        /// Use this mathod in order to know if this parser can process or not a specific user-input.
        /// In addition, for performance speed-up, it will compute in advance the reply for the user.
        /// </summary>
        /// <param name="input">Message sent by the user</param>
        /// <returns>Returns True if it can handle the message or Flase on the contrary</returns>
        public async Task<Boolean> haveAnswer(string activityInput)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string path = $"{root}{Path.DirectorySeparatorChar}Parser{Path.DirectorySeparatorChar}parser_dictionary.txt";
            Dictionary<string, string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
            string input = activityInput.ToLower();

            foreach (KeyValuePair<string, string> entry in dict)
            {
                if (userData.GetProperty<bool>(entry.Key))
                {
                    BotState botState = new BotState(stateClient);
                    BotData botData = await botState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                    string propertyName = entry.Key + "State";
                    ManagerEnum enumValue = StringToEnum.convertToEnum(entry.Key);
                    this.replyManager = ReplyManagerFactory.genererateReplyManager(this.activity, input, enumValue);
                    Activity replyToSend = await getRightActivity(enumValue, botData, propertyName);
                    if(replyToSend == null)
                    {
                        reply = activity.CreateReply("I'm so Sorry, something went wrong in parsing your request");
                    }
                    else
                    {
                        reply = replyToSend;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method is used to retrive the correct stateProperty from the botData and to process and save the answer
        /// in order to speed-up the overall performance.
        /// </summary>
        /// <param name="enumValue">Is used in order to understand which property is needed</param>
        /// <param name="botData">BotData retrieve from user message</param>
        /// <param name="propertyName">Property name to search for</param>
        /// <returns>Returns the activity get from the replyManager or null if something wrong happened</returns>
        private async Task<Activity> getRightActivity(ManagerEnum enumValue, BotData botData, string propertyName){
            Activity replyToSend = new Activity();
            switch (enumValue)
            {
                case ManagerEnum.SearchMovie :
                    SearchMovieState state = botData.GetProperty<SearchMovieState>(propertyName);
                    replyToSend = await replyManager.getResponseWithState<SearchMovieState>(state);
                    break;
                case ManagerEnum.SearchCinema:
                    SearchCinemaState state2 = botData.GetProperty<SearchCinemaState>(propertyName);
                    replyToSend = await replyManager.getResponseWithState<SearchCinemaState>(state2);
                    break;
                default :
                    replyToSend = null;
                    break;    
            }
            return replyToSend;
        } 
    }
}