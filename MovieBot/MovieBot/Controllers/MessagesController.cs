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
                //Inizializtion af all the needed Parses
                AbstractParser parserText = new MessageTextParser(activity);
                AbstractParser parserLUIS = new LUISParser(activity);
                MessageStateParser stateParser = new MessageStateParser(activity);

                String userInput;
                string audioString= String.Empty;
                Activity reply;

                if(activity.Attachments != null)
                {
                    if (activity.Attachments.Count > 0)
                    {
                        /// Here the <see cref="MessagesController"/> can handle messages with <see cref="Attachment"/>
                        foreach (Attachment item in activity.Attachments)
                        {
                            if (item.ContentType.Contains("audio"))
                            {
                                if (item.ContentType.Contains("ogg"))
                                {
                                    audioString = SpeechRecognitionUtility.DoSpeechReco(activity.Attachments.First());
                                }
                                else
                                {
                                    reply = activity.CreateReply("Sorry, at the moment I can only understand audio in the .wav format. I hope my programmer will make me smarter ;)");
                                    APIResponse audioResult = await connector.Conversations.ReplyToActivityAsync(reply);
                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }                      
                            }
                        }
                    }
                }

                if (audioString.Equals(String.Empty))
                {
                    userInput = activity.Text.ToLower();
                }
                else
                {
                    userInput = audioString;
                }

                //Retrieve the answer from the correct Parser
                if (parserText.haveAnswer(userInput))
                {
                    reply = await parserText.computeParsing();
                }
                else if (await stateParser.haveAnswer(userInput))
                {
                    reply = stateParser.computeParsing();
                }
                else if (parserLUIS.haveAnswer(userInput))
                {
                    reply = await parserLUIS.computeParsing();
                }
                else
                {
                    //standard Reply if the Bot cannot handle the User request
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
            Activity reply = activity.CreateReply("Hi!");

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
                ReplyManager manager = new StartMessageReplyManager(activity, "");
                reply = await manager.getResponse();
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                ReplyManager manager = new StartMessageReplyManager(activity, "");
                reply = await manager.getResponse();
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
                ReplyManager manager = new StartMessageReplyManager(activity, "");
                reply = await manager.getResponse();
            }

            return reply;
        }
    }
}