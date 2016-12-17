using MovieBot.ReplyManagers;
using MovieBot.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace MovieBot.Parser
{
    public static class ParserUtitlity
    {
        public static ManagerEnum getManagerFromInput(string input)
        {
            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string path = $"{root}{Path.DirectorySeparatorChar}Utility{Path.DirectorySeparatorChar}parser_dictionary.txt";
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
                    return enumValue;
                }
            }
            return ManagerEnum.Default;
        }
    }
}