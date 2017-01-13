using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.ReplyManagers
{
    /// <summary>
    /// Enum that contains all the Type of <see cref="ReplyManager"/>
    /// </summary>
    public enum ManagerEnum
    {
        SearchMovie,
        SearchCinema,
        AllProjections,
        Help,
        Default
    }
}