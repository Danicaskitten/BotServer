using Microsoft.Bot.Connector;
using MovieBot.States;
using MovieBot.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MovieBot.Utility;

namespace MovieBot.ReplyManagers
{
    public class SearchCinemaReplyManager : ReplyManager
    {
        public SearchCinemaReplyManager(Activity activity, string input) : base(activity, input){ }

        public override async Task<Activity> getResponse()
        {
            SearchCinemaState state = new SearchCinemaState
            {
                ChannelType = activity.ChannelId,
                UserID = activity.From.Id,
                StateNum = 0
            };

            StateReply stateReplay = state.getReplay(input);
            Activity replyToConversation = new Activity();
            if (stateReplay != null)
            {
                StateClient stateClient = activity.GetStateClient();
                replyToConversation = await this.parseStateReply<SearchCinemaState>(stateReplay, stateClient, state, "SearchCinema");
            }
            else
            {
                replyToConversation = activity.CreateReply("Something went wrong in the parsing of your last message. Please try sending again your Search Cinema request"); 
            }
            return replyToConversation;
        }

        public override async Task<Activity> getResponseWithState<T>(T stateInput)
        {
            if (typeof(T) == typeof(SearchCinemaState))
            {
                T temp = (T)(object)stateInput;
                SearchCinemaState state = (SearchCinemaState)(object)stateInput;
                StateReply stateReplay = state.getReplay(input);
                Activity replyToConversation = new Activity();
                if (stateReplay != null)
                {
                    StateClient stateClient = activity.GetStateClient();
                    replyToConversation = await this.parseStateReply<SearchCinemaState>(stateReplay, stateClient, state, "SearchCinema");
                }
                else
                {
                    replyToConversation = activity.CreateReply("Something went wrong in the parsing of your last message. Please try sending again your Search Cinema request");
                }
                return replyToConversation;
            }
            else
            {
                Activity replyToConversation = activity.CreateReply("I'm so sorry, I didn't manage to process your request. Please try sending again your Search Cinema request");
                return replyToConversation;
            }
        }
    }
}