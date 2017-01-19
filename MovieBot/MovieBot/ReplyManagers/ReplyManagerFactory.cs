using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.ReplyManagers
{
    /// <summary>
    /// Factory that can generate all the available <see cref="ReplyManager"/>
    /// </summary>
    public static class ReplyManagerFactory
    {
        /// <summary>
        /// This method returns the desidered <see cref="ReplyManager"/>
        /// </summary>
        /// <param name="activity">User <see cref="Activity"/></param>
        /// <param name="input">User input</param>
        /// <param name="requestedManager"><see cref="ManagerEnum"/> that contains the requested <see cref="ReplyManager"/></param>
        /// <returns>The requested <see cref="ReplyManager"/></returns>
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
                case ManagerEnum.AllProjections:
                    return new SearchAllProjectionsReplyManager(activity, input);
                case ManagerEnum.Start:
                    return new StartMessageReplyManager(activity, input);
                default:
                    return null;
            }
        } 
    }
}