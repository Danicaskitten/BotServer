using Microsoft.Bot.Connector;
using MovieBot.States;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieBot.ReplayManager
{
    public static class SearchMovie
    {
        public static async Task<Activity> getResponse(Activity activity, string input)
        {
            SearchCinemaState state = new SearchCinemaState(activity.ChannelId, activity.From.Id);
            StateReply stateReplay= state.getReplay(input);
            StateClient stateClient = activity.GetStateClient();
            if (!(stateReplay.IsFinalState))
            {
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                userData.SetProperty<bool>("searchMovie", true);
                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                BotState botState = new BotState(stateClient);
                BotData botData = new BotData(eTag: "*");
                botData.SetProperty<SearchCinemaState>("SearchState", state);
                BotData response = await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, botData);
            }
            else
            {
                await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
            }
            Activity backActivity = activity.CreateReply(stateReplay.GetReplayMessage);
            return backActivity;
        }
    }
}