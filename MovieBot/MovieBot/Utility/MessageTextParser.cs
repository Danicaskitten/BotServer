using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

namespace MovieBot.Utility
{
    public static class MessageTextParser
    {
        public static void startParsing(ConnectorClient connector, Activity activity)
        {
            //TODO implementare la pate di parsing utilizzando un dizionario del tipo {"search by moovie": "regexpr corrispondente"} così che poi posso chiamare una factory che mi restituisce la classe giusta in base al primo pezzo
            //TODO creare la factory
            //TODO implementare il dictionary
            //TODO eseguire un for per ogni elemento del dictionary

            string input = activity.Text.ToLower();
            string sPattern = "(search movie|searchmovie)";
            if(System.Text.RegularExpressions.Regex.IsMatch(input, sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {

            }
        }
    }
}