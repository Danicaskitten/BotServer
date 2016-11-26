using System;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using MovieBot.ReplayManager;
using System.Net.Http;
using System.Threading.Tasks;


namespace MovieBot.Utility
{
    public static class MessageTextParser
    {
        public static async Task<APIResponse> computeParsing(ConnectorClient connector, Activity activity)
        {
            //TODO implementare la pate di parsing utilizzando un dizionario del tipo {"search by moovie": "regexpr corrispondente"} così che poi posso chiamare una factory che mi restituisce la classe giusta in base al primo pezzo
            //TODO creare la factory
            //TODO implementare il dictionary
            //TODO eseguire un for per ogni elemento del dictionary

            string input = activity.Text.ToLower();
            string sPattern = "(search movie|searchmovie)";
            Activity reply = new Activity();

            if (System.Text.RegularExpressions.Regex.IsMatch(input, sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                string pattern = "(search movie |/searchmovie)";
                string replacement = string.Empty;
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(input, replacement);
                reply = await SearchMovie.getResponse(activity, result);
            }

            APIResponse response = await connector.Conversations.ReplyToActivityAsync(reply);
            return response;
        }
    }
}