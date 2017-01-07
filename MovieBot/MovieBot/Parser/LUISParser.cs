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
            ParserObject parsed = getManagerFromInput(inputComputed);
            ManagerEnum enumResult = parsed.ReplyManagerEnum;
            string parsedInput = parsed.ParsedInput;
            if (enumResult.Equals(ManagerEnum.Default))
            {
                return false;
            }
            else
            {
                this.replyManager = ReplyManagerFactory.genererateReplyManager(activity, parsedInput, enumResult);
                return true;
            }
        }

        private string computeLUISOutput(LUISResponse response)
        {
            string intent = response.topScoringIntent.intent;
            List<Contract.LUIS.Entity> entities = response.entities;
            string newUserInput = "";
            if (entities.Count > 0)
            {
                string input = entities.First().entity;

                switch (intent)
                {
                    case "SearchMovie":
                        newUserInput = "search movie " + input;
                        break;
                    case "SearchCinema":
                        newUserInput = "search cinema " + input;
                        break;
                    case "Help":
                        newUserInput = "help";
                        break;
                }
            }
            else
            {
                if (intent.Equals("AllProjections"))
                {
                    newUserInput = "all projections";
                }
            }

            return newUserInput;
        }
    }
}