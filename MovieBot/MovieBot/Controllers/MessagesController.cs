using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using MovieBot.ReplyManagers;
using MovieBot.Parser;
using MovieBot.Utility.Speech;

namespace MovieBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.Message)
            {
                //TODO add the LUIS parser (magari fare un bel ciclo)
                AbstractParser parserText = new MessageTextParser(activity, connector);
                AbstractParser parserLUIS = new LUISParser(activity, connector);
                MessageStateParser stateParser = new MessageStateParser(activity, connector);

                String userInput;
                Activity reply;

                if (activity.Attachments.Count > 0)
                {
                    userInput = SpeechRecognitionUtility.DoSpeechReco(activity.Attachments.First());
                }
                else
                {
                    userInput = activity.Text.ToLower();
                }

                if (parserText.haveAnswer(activity.Text.ToLower()))
                {
                    reply = await parserText.computeParsing();
                }
                else if (await stateParser.haveAnswer(activity.Text.ToLower()))
                {
                    reply = stateParser.computeParsing();
                }
                else if (parserLUIS.haveAnswer(activity.Text.ToLower()))
                {
                    reply = await parserLUIS.computeParsing();
                }
                else
                {
                    reply = activity.CreateReply("Sorry, I did not understand your request. Please ask me for Help in order to know my functionalities");
                } 
                APIResponse result = await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                Activity reply = await HandleSystemMessage(activity);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        /// <summary>
        /// This method is used to handle all those non message activities that the bot can recieve.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            Activity reply = new Activity();

            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                ReplyManager manager = new StartMessageReplyManager(activity, activity.Text.ToLower());
                reply = await manager.getResponse();
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                ReplyManager manager = new StartMessageReplyManager(activity, activity.Text.ToLower());
                reply = await manager.getResponse();
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
            }

            return reply;
        }
    }
}