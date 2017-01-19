using MovieBot.ReplyManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Utility
{
    /// <summary>
    /// This class contain useful method used by <see cref="ReplyManager"/>
    /// </summary>
    public static class StringToEnum
    {   
        public static ManagerEnum convertToEnum (string stringToConvert)
        {
            switch (stringToConvert)
            {
                case ("SearchMovie"):
                    return ManagerEnum.SearchMovie;
                case ("SearchCinema"):
                    return ManagerEnum.SearchCinema;
                case ("AllProjections"):
                    return ManagerEnum.AllProjections;
                case ("Start"):
                    return ManagerEnum.Start;
                default:
                    return ManagerEnum.Default;
            }
        }
    }
}