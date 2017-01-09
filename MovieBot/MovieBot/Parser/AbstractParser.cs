using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MovieBot.ReplyManagers;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using MovieBot.Utility;

namespace MovieBot.Parser
{
    /// <summary>
    /// This abstract class is the model for all the parser in this project
    /// </summary>
    public abstract class AbstractParser
    {
        protected Activity activity;
        protected ConnectorClient connector;
        protected ReplyManager replyManager;
        protected bool audioFlag;

        /// <summary>
        /// This is the basic constructor for a Parser
        /// </summary>
        /// <param name="activity">User Activity</param>
        /// <param name="connector">Generated ConnectorClient</param>
        /// <param name="audioFlag">true if the User send an audio file, false otherwise</param>
        public AbstractParser(Activity activity, ConnectorClient connector,bool audioFlag)
        {
            this.activity = activity;
            this.connector = connector;
            this.audioFlag = audioFlag;
        }
        /// <summary>
        /// This method is used to effectively analyze the incoming message in order to produce the
        /// correct responce.It will return the activity that needs to be sent back to the user.
        /// </summary>
        /// <returns>
        /// This method returns the activity or null if something go wrong in the parsing process
        /// </returns>
        public abstract Task<Activity> computeParsing();
        /// <summary>
        /// Use this mathod in order to know if this parser can process or not a specific user-input
        /// </summary>
        /// <param name="input">Message sent by the user</param>
        /// <returns>Returns True if it can handle the message or Flase on the contrary</returns>
        public abstract Boolean haveAnswer(string input);
        /// <summary>
        /// This method can be used in order to get the the parsed input sent by the user and the ManagerEnum
        /// reference in order to know which is the ReplyManager desidered by the user.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>a ParserObject that contains all the information for the process of the user request</returns>
        protected ParserObject getManagerFromInput(string input)
        {
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string path = $"{root}{Path.DirectorySeparatorChar}Parser{Path.DirectorySeparatorChar}parser_dictionary.txt";
            Dictionary<string, string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
            string lowerInput = input.ToLower();

            foreach (KeyValuePair<string, string> entry in dict)
            {
                string pattern = entry.Value;
                if (System.Text.RegularExpressions.Regex.IsMatch(lowerInput, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    string replacement = string.Empty;
                    Regex rgx = new Regex(pattern);
                    string result = rgx.Replace(lowerInput, replacement);
                    ManagerEnum enumValue = StringToEnum.convertToEnum(entry.Key);
                    ParserObject innerReturnValue = new ParserObject
                    {
                        ParsedInput = result,
                        ReplyManagerEnum = enumValue
                    };
                    return innerReturnValue;
                }
            }

            ParserObject returnValue = new ParserObject();

            if (lowerInput.Contains("help"))
            {
                returnValue = new ParserObject
                {
                    ParsedInput = input,
                    ReplyManagerEnum = ManagerEnum.Help
                };
            }
            else
            {
                returnValue = new ParserObject
                {
                    ParsedInput = input,
                    ReplyManagerEnum = ManagerEnum.Default
                };
            }
            return returnValue;
        }
    }
}