using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using MovieBot.Contract.LUIS;
using MovieBot.Utility;
using MovieBot.ReplyManagers;

namespace MovieBot.Parser
{
    public class LUISParser : AbstractParser
    {
        public LUISParser(Activity activity, ConnectorClient connector) : base(activity, connector){}

        public override async Task<Activity> computeParsing()
        {
            Activity reply = await this.replyManager.getResponse();
            return reply;
        }

        public override bool haveAnswer(string input)
        {
            LUISResponse response = LuisUtility.GetEntityFromLUIS(input);
            string inputComputed = computeLUISOutput(response);
            ManagerEnum enumResult = getManagerFromInput(inputComputed);
            if (enumResult.Equals(ManagerEnum.Default))
            {
                this.replyManager = ReplyManagerFactory.genererateReplyManager(activity, inputComputed, enumResult);
                return false;
            }
            else
            {
                return true;
            }
        }

        private string computeLUISOutput(LUISResponse response)
        {
            string intent = response.topScoringIntent.intent;
            List<Contract.LUIS.Entity> entities = response.entities;
            string input = entities.First().entity;

            switch (intent)
            {
                case "SearchMovie":
                    string newUserInput = "search movie " + input;
                    return newUserInput;
            }

            return "";
        }
    }
}