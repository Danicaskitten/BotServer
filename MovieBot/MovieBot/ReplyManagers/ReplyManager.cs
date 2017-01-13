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
    /// <summary>
    /// The ReplyManager is the class that can handle <see cref="SearchState"/> and <see cref="StateReply"/> 
    /// </summary>
    public abstract class ReplyManager
    {
        /// <summary>
        /// <see cref="Activity"/> sent by the user
        /// </summary>
        protected Activity activity;
        protected string userInput;

        /// <summary>
        /// Base constructor for a <see cref="ReplyManager"/>
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> sent by the user</param>
        /// <param name="input">User input message</param>
        public ReplyManager(Activity activity, string input)
        {
            this.activity = activity;
            this.userInput = input;
        }

        /// <summary>
        /// This method parses the userInput, generates the right <see cref="SearchState"/> and saves it in the Bot Framework State Service
        /// </summary>
        /// <returns>The <see cref="Activity"/> to send back to the User</returns>
        public abstract Task<Activity> getResponse();

        /// <summary>
        /// This method takes as parameter the previous <see cref="SearchState"/>, parses the userInput, update the state and saves it back in the Bot Framework State Service
        /// </summary>
        /// <typeparam name="T">Type of <see cref="SearchState"/></typeparam>
        /// <param name="state">Old <see cref="SearchState"/> to update</param>
        /// <returns>The <see cref="Activity"/> to send back to the User</returns>
        public abstract Task<Activity> getResponseWithState<T>(T state);

        /// <summary>
        /// This method gets the <see cref="StateReply"/>, parses it and, if it is necessary, saves it back in the Bot Framework
        /// </summary>
        /// <typeparam name="T">Type of <see cref="SearchState"/></typeparam>
        /// <param name="stateReplay"><see cref="StateReply"/> returned by the <see cref="SearchState"/></param>
        /// <param name="stateClient"><see cref="StateClient"/> that needs to be updated with the new state</param>
        /// <param name="state"><see cref="SearchState"/> to send</param>
        /// <param name="dataProperty">Name of the property that needs to be set in the <see cref="StateClient"/></param>
        /// <returns>The <see cref="Activity"/> to send back to the User</returns>
        protected async Task<Activity> parseStateReply<T>(StateReply stateReplay, StateClient stateClient, T state, String dataProperty)
        {
            //If the State is Final then is deleted from the client and the property is updated
            if (!(stateReplay.IsFinalState))
            {
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                userData.SetProperty<bool>(dataProperty, true);

                userData.SetProperty<T>(dataProperty +"State", state);
                BotData response = await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
            }
            else
            {
                //The client is updated with the new state
                await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
            }

            Activity replyToConversation = activity.CreateReply(stateReplay.GetReplayMessage);

            Activity replyParsed = manageAttachment(replyToConversation, stateReplay);

            return replyToConversation;
        }

        /// <summary>
        /// This method can Handle <see cref="Attachment"/> and returns the right <see cref="Activity"/>
        /// </summary>
        /// <param name="replyToConversation"><see cref="Activity"/> to sent back to the user</param>
        /// <param name="replyFromState"><see cref="StateReply"/> to parse</param>
        /// <returns>The <see cref="Activity"/> to send back to the User</returns>
        private Activity manageAttachment(Activity replyToConversation, StateReply replyFromState)
        {
            string special = replyFromState.GetSpecial;
            switch (special){
                case "herocard":
                    HeroCard heroGet = replyFromState.HeroCard;
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.Attachments = new List<Attachment>();

                    Attachment plAttachment = heroGet.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                    break;
            }

            return replyToConversation;
        }
    }
}