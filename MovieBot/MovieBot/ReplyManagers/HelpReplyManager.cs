using Microsoft.Bot.Connector;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MovieBot.ReplyManagers
{
    public class HelpReplyManager : ReplyManager
    {
        public HelpReplyManager(Activity activity, string input) : base(activity, input) { }

        public override async Task<Activity> getResponse()
        {
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string start_message = System.IO.File.ReadAllText($"{root}{Path.DirectorySeparatorChar}StandardReplies{Path.DirectorySeparatorChar}help_message.txt");
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