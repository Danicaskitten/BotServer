using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;
using MovieBot.Contract.LUIS;
using MovieBot.Utility;

namespace MovieBot.Parser
{
    public class LUISParser : Parser
    {
        public LUISParser(Activity activity, ConnectorClient connector) : base(activity, connector){}

        public override Task<Activity> computeParsing()
        {
            LUISResponse response = LuisUtility.GetEntityFromLUIS(activity.Text.ToLower());
            throw new NotImplementedException();
        }

        public override bool haveAnswer(string input)
        {
            throw new NotImplementedException();
        }
    }
}