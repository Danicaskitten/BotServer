﻿using Microsoft.Bot.Connector;
using MovieBot.States;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieBot.ReplyManagers
{
    public class SearchAllProjectionsReplyManager : ReplyManager
    {
        /// <inheritdoc />
        public SearchAllProjectionsReplyManager(Activity activity, string input) : base(activity, input){ }

        /// <inheritdoc />
        public override async Task<Activity> getResponse()
        {
            //creation of the right State
            SearchAllProjectionsState state = new SearchAllProjectionsState
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
                replyToConversation = await this.parseStateReply<SearchAllProjectionsState>(stateReplay, stateClient, state, "AllProjections");
            }
            else
            {
                replyToConversation = activity.CreateReply("Something went wrong in the parsing of your last message. Please try sending again your Search Cinema request");
            }
            return replyToConversation;
        }

        /// <inheritdoc />
        public override async Task<Activity> getResponseWithState<T>(T stateInput)
        {
            if (typeof(T) == typeof(SearchAllProjectionsState))
            {
                T temp = (T)(object)stateInput;
                SearchAllProjectionsState state = (SearchAllProjectionsState)(object)stateInput;
                StateReply stateReplay = state.getReplay(userInput);
                Activity replyToConversation = new Activity();
                if (stateReplay != null)
                {
                    StateClient stateClient = activity.GetStateClient();
                    replyToConversation = await this.parseStateReply<SearchAllProjectionsState>(stateReplay, stateClient, state, "AllProjections");
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