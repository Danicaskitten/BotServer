using MovieBot.ReplyManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Parser
{
    public class ParserObject
    {
        public string ParsedInput { get; set; }

        public ManagerEnum ReplyManagerEnum { get; set; }
    }
}