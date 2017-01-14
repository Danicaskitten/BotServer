using Microsoft.Bot.Connector;
using MovieBot.States;
using MovieBot.Parser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MovieBot.Utility;

namespace MovieBot.ReplyManagers
{
    public class SearchMovieReplyManager : ReplyManager
    {
        /// <inheritdoc />
        public SearchMovieReplyManager(Activity activity, string input) : base(activity, input){}

        /// <inheritdoc />
        public override async Task<Activity> getResponse()
        {
            //creation of the rigth state
            SearchMovieState state = new SearchMovieState
            {
                ChannelType = activity.ChannelId,
                UserID = activity.From.Id,
                StateNum = 0
            };

            StateReply stateReplay = state.getReplay(userInput);
            Activity replyToConversation = new Activity();
            if (stateReplay != null)
            {
                StateClient stateClient = activity.GetStateClient();
                replyToConversation = await this.parseStateReply<SearchMovieState>(stateReplay, stateClient, state, "SearchMovie");
            }
            else
            {
                replyToConversation = activity.CreateReply("Something went wrong in the parsing of your last message. Please try sending again your Search Movie request");
            }
            return replyToConversation;
        }

        /// <inheritdoc />
        public override async Task<Activity> getResponseWithState<T>(T stateInput)
        {
            if(typeof(T) == typeof(SearchMovieState))
            {
                T temp = (T)(object)stateInput;
                SearchMovieState state = (SearchMovieState)(object)stateInput;
                StateReply stateReplay = state.getReplay(userInput);
                Activity replyToConversation = new Activity();
                if (stateReplay != null)
                {
                    StateClient stateClient = activity.GetStateClient();
                    replyToConversation = await this.parseStateReply<SearchMovieState>(stateReplay, stateClient, state, "SearchMovie");
                }
                else
                {
                    replyToConversation = activity.CreateReply("Something went wrong in the parsing of your last message. Please try sending again your Search Movie request");
                }
                return replyToConversation;
            }
            else
            {
                Activity replyToConversation = activity.CreateReply("I'm so sorry, I didn't manage to process your request");
                return replyToConversation;
            }
        }
    }
}