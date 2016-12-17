using System;
using Microsoft.Bot.Connector;
using MovieBot.ReplyManagers;
using System.Threading.Tasks;

namespace MovieBot.Parser
{
    public class MessageTextParser : Parser
    {
        /// <inheritdoc />
        public MessageTextParser(Activity activity, ConnectorClient connector) : base(activity, connector){}
        
        /// <inheritdoc />
        public override bool haveAnswer(string activityInput)
        {
            ManagerEnum enumResult = ParserUtitlity.getManagerFromInput(activityInput);
            if (enumResult.Equals(ManagerEnum.Default))
            {
                this.replyManager = ReplyManagerFactory.genererateReplyManager(activity,activityInput,enumResult);
                return false;
            }
            else
            {
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