using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using MovieBot.Contract.LUIS;
using MovieBot.Utility;
using MovieBot.ReplyManagers;

namespace MovieBot.Parser
{
    /// <summary>
    /// <see cref="AbstractParser"/> that can be used to manage LUIS requests
    /// </summary>
    public class LUISParser : AbstractParser
    {
        /// <inheritdoc />
        public LUISParser(Activity activity) : base(activity){}

        /// <inheritdoc />
        public override async Task<Activity> computeParsing()
        {
            Activity reply = await this.replyManager.getResponse();
            return reply;
        }

        /// <inheritdoc />
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

        /// <summary>
        /// This method is used in order to convert a <see cref="LUISResponse"/> into the correct command
        /// </summary>
        /// <param name="response"><see cref="LUISResponse"/> retrived from the LUIS API</param>
        /// <returns>The command deisdered by the User</returns>
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