using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;

namespace MovieBot.ReplyManagers
{
    public class StartMessageReplyManager : ReplyManager
    {
        public StartMessageReplyManager(Activity activity, string input) : base(activity, input) { }

        public override async Task<Activity> getResponse()
        {
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string start_message = System.IO.File.ReadAllText($"{root}{Path.DirectorySeparatorChar}StandardReplies{Path.DirectorySeparatorChar}start_message.txt");
            Activity reply = activity.CreateReply(start_message);
            await Task.Delay(1);
            return reply;
        }

        public override Task<Activity> getResponseWithState<T>(T state)
        {
            throw new NotImplementedException();
        }
    }
}