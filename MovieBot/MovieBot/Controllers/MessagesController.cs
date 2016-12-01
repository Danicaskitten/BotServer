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
using MovieBot.Utility;

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
                // calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                //await connector.Conversations.ReplyToActivityAsync(reply);
                Parser parser = new MessageTextParser(activity, connector);
                MessageStateParser stateParser = new MessageStateParser(activity, connector);
                Activity reply;
                if (await stateParser.haveAnswer(activity.Text.ToLower()))
                {
                    reply = await stateParser.computeParsing();
                }
                else if (parser.haveAnswer(activity.Text.ToLower()))
                {
                    reply = await parser.computeParsing();
                }
                else
                {
                    reply = activity.CreateReply("Sorry, due to internal error, I can't manage your request");
                } 
                APIResponse result = await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                Activity reply = HandleSystemMessage(activity);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity activity)
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
                string startMessage = StartMessage.getStartMessage();
                reply = activity.CreateReply(startMessage);
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string startMessage = StartMessage.getStartMessage();
                reply = activity.CreateReply(startMessage);
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