using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MovieBot.ReplyManagers
{
    public static class StartMessage
    {
        public static string getStartMessage()
        {
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string start_message= System.IO.File.ReadAllText($"{root}{Path.DirectorySeparatorChar}StandardReplies{Path.DirectorySeparatorChar}start_message.txt");
            
            return start_message;
        }
    }
}