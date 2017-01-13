using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace MovieBot.ReplyManagers
{
    public class StartMessageReplyManager : ReplyManager
    {
        /// <inheritdoc />
        public StartMessageReplyManager(Activity activity, string input) : base(activity, input) { }

        /// <inheritdoc />
        public override async Task<Activity> getResponse()
        {
            //Here the method retrieves the reply from the File
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string start_message = System.IO.File.ReadAllText($"{root}{Path.DirectorySeparatorChar}StandardReplies{Path.DirectorySeparatorChar}start_message.txt");
            Activity reply = activity.CreateReply(start_message);
            //Necessary to make the method async
            await Task.Delay(1);
            return reply;
        }

        /// <inheritdoc />
        public override Task<Activity> getResponseWithState<T>(T state)
        {
            throw new NotImplementedException();
        }
    }
}