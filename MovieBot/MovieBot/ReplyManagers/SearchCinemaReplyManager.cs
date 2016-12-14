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
    public class SearchCinemaReplyManager : ReplyManager
    {
        public SearchCinemaReplyManager(Activity activity, string input) : base(activity, input){ }

        public override async Task<Activity> getResponse()
        {
            SearchMovieState state = new SearchMovieState
            {
                ChannelType = activity.ChannelId,
                UserID = activity.From.Id,
                ChoosenCinema = false
            };

            StateReply stateReplay = state.getReplay(input);
            if(stateReplay != null)
            {
                StateClient stateClient = activity.GetStateClient();
                if (!(stateReplay.IsFinalState))
                {
                    BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                    userData.SetProperty<bool>("searchMovie", true);
                    //await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                    userData.SetProperty<SearchMovieState>("SearchState", state);
                    BotData response = await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                }
                else
                {
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                }

                Activity replyToConversation = activity.CreateReply(stateReplay.GetReplayMessage);
                return replyToConversation;
            }
            else
            {
                Activity replyToConversation = activity.CreateReply("Something went wrong in the message parsing, please try to restart your request");
                return replyToConversation;
            }
            
        }

        public override async Task<Activity> getResponseWithState<T>(T stateInput)
        {
            if (typeof(T) == typeof(SearchCinemaState))
            {
                T temp = (T)(object)stateInput;
                SearchCinemaState state = (SearchCinemaState)(object)stateInput;
                StateReply stateReplay = state.getReplay(input);
                if (stateReplay != null)
                {
                    StateClient stateClient = activity.GetStateClient();
                    if (!(stateReplay.IsFinalState))
                    {
                        BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                        userData.SetProperty<bool>("SearchMovie", true);
                        //await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                        userData.SetProperty<SearchState>("SearchMovieState", state);
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
                else
                {
                    Activity replyToConversation = activity.CreateReply("Something went wrong in the message parsing, please try to restart your request");
                    return replyToConversation;
                }
            }
            else
            {
                Activity replyToConversation = activity.CreateReply("I'm so sorry, I didn't manage to process your reqeust");
                return replyToConversation;
            }
        }
    }
}