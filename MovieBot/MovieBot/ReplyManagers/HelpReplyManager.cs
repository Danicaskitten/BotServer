﻿using Microsoft.Bot.Connector;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MovieBot.ReplyManagers
{
    /// <summary>
    /// <see cref="ReplyManager"/> that manages the Help request form the user
    /// </summary>
    public class HelpReplyManager : ReplyManager
    {
        /// <inheritdoc />
        public HelpReplyManager(Activity activity, string input) : base(activity, input) { }

        /// <inheritdoc />
        public override async Task<Activity> getResponse()
        {
            //Here the method retrieves the reply from the File
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string help_message = System.IO.File.ReadAllText($"{root}{Path.DirectorySeparatorChar}StandardReplies{Path.DirectorySeparatorChar}help_message.txt");
            Activity reply = activity.CreateReply(help_message);
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