using MovieBot.ReplyManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Utility
{
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
                default:
                    return ManagerEnum.Default;
            }
        }
    }
}