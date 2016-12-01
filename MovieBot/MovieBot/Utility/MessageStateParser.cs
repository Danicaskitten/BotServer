using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using MovieBot.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieBot.Utility
{
    public class MessageStateParser
    {
        private SearchCinemaState state;
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
            string input = activity.Text.ToLower();
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            if (userData.GetProperty<bool>("searchMovie"))
            {
                BotState botState = new BotState(stateClient);
                BotData botData = await botState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                state = botData.GetProperty<SearchCinemaState>("SearchState");
                this.replyManager = ReplyManagerFactory.genererateReplyManager(activity, input, ManagerEnum.SearchMovie);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}