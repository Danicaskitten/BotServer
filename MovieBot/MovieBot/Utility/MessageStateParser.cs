using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using MovieBot.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace MovieBot.Utility
{
    public class MessageStateParser
    {
        private SearchState state;
        private Activity activity;
        private ConnectorClient connector;
        private ReplyManager replyManager;

        public MessageStateParser(Activity activity, ConnectorClient connector){
            this.activity = activity;
            this.connector = connector;
        }

        public async Task<Activity> computeParsing()
        {
            Activity reply = await replyManager.getResponseWithState(state);
            return reply;
        }

        public async Task<Boolean> haveAnswer(string activityInput)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string path = $"{root}{Path.DirectorySeparatorChar}Utility{Path.DirectorySeparatorChar}parser_dictionary.txt";
            Dictionary<string, string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
            string input = activityInput.ToLower();

            foreach (KeyValuePair<string, string> entry in dict)
            {
                if (userData.GetProperty<bool>(entry.Key))
                {
                    BotState botState = new BotState(stateClient);
                    BotData botData = await botState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                    string propertyName = entry.Key + "State";
                    state = botData.GetProperty<SearchState>(propertyName);
                    ManagerEnum enumValue = StringToEnum.convertToEnum(entry.Key);
                    this.replyManager = ReplyManagerFactory.genererateReplyManager(this.activity, input, enumValue);
                    return true;
                }
            }
            //Activity reply = activity.CreateReply("Sorry, I am only an alpha prototype. So I reply only to the command \"search movie Title\"");
            return false;
        }
    }
}