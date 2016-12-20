using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.ReplyManagers
{
    public static class ReplyManagerFactory
    {
        public static ReplyManager genererateReplyManager(Activity activity,string input, ManagerEnum requestedManager)
        {
            switch (requestedManager)
            {
                case ManagerEnum.SearchMovie:
                    return new SearchMovieReplyManager(activity, input);
                case ManagerEnum.SearchCinema:
                    return new SearchCinemaReplyManager(activity, input);
                case ManagerEnum.Help:
                    return new HelpReplyManager(activity, input);
                default:
                    return null;
            }
        } 
    }
}