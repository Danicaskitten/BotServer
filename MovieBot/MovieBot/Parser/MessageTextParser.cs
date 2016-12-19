using System;
using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using System.Threading.Tasks;

namespace MovieBot.Parser
{
    public class MessageTextParser : AbstractParser
    {
        /// <inheritdoc />
        public MessageTextParser(Activity activity, ConnectorClient connector) : base(activity, connector){}
        
        /// <inheritdoc />
        public override bool haveAnswer(string activityInput)
        {
            ParserObject parsed = getManagerFromInput(activityInput);
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

        /// <inheritdoc />
        public override async Task<Activity> computeParsing()
        {
            Activity reply = await this.replyManager.getResponse();  
            return reply;
        }
    }
}