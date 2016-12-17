using System;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using MovieBot.ReplyManagers;
using MovieBot.Utility;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;

namespace MovieBot.Parser
{
    public class MessageTextParser : Parser
    {
        /// <inheritdoc />
        public MessageTextParser(Activity activity, ConnectorClient connector) : base(activity, connector){}
        
        /// <inheritdoc />
        public override bool haveAnswer(string activityInput)
        {
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string path = $"{root}{Path.DirectorySeparatorChar}Utility{Path.DirectorySeparatorChar}parser_dictionary.txt";
            Dictionary<string,string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
            string input = activityInput.ToLower();

            foreach (KeyValuePair<string,string> entry in dict)
            {
                string pattern = entry.Value;
                if (System.Text.RegularExpressions.Regex.IsMatch(input, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    string replacement = string.Empty;
                    Regex rgx = new Regex(pattern);
                    string result = rgx.Replace(input, replacement);
                    ManagerEnum enumValue = StringToEnum.convertToEnum(entry.Key);
                    this.replyManager = ReplyManagerFactory.genererateReplyManager(this.activity, result, enumValue);
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public override async Task<Activity> computeParsing()
        {
            Activity reply = await this.replyManager.getResponse();  
            return reply;
        }
    }
}