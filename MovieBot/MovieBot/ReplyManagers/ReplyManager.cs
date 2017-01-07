using Microsoft.Bot.Connector;
using MovieBot.States;
using MovieBot.Utility;
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
        public abstract Task<Activity> getResponseWithState<T>(T state);

        protected async Task<Activity> parseStateReply<T>(StateReply stateReplay, StateClient stateClient, T state)
        {
            if (!(stateReplay.IsFinalState))
            {
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                userData.SetProperty<bool>("SearchCinema", true);

                userData.SetProperty<T>("SearchCinemaState", state);
                BotData response = await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
            }
            else
            {
                await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
            }

            Activity replyToConversation = activity.CreateReply(stateReplay.GetReplayMessage);

            if (stateReplay.GetSpecial == "herocard")
            {
                HeroCard heroGet = stateReplay.HeroCard;
                replyToConversation.Recipient = activity.From;
                replyToConversation.Type = "message";
                replyToConversation.Attachments = new List<Attachment>();

                Attachment plAttachment = heroGet.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }
            return replyToConversation;
        }
    }
}