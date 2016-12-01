using System;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using MovieBot.ReplyManagers;
using System.Net.Http;
using System.Threading.Tasks;


namespace MovieBot.Utility
{
    public class MessageTextParser : Parser
    {
        public MessageTextParser(Activity activity, ConnectorClient connector) : base(activity, connector){}

        public override bool haveAnswer(string activityInput)
        {
            //TODO mettere qui la parte di ricerca con le regular expression e poi chiamare la factory
            // in questo modo serve aggiungere alla classe parser un attributo di tipo ReplyManager
            //TODO implementare la pate di parsing utilizzando un dizionario del tipo {"search by moovie": "regexpr corrispondente"} così che poi posso chiamare una factory che mi restituisce la classe giusta in base al primo pezzo
            //TODO creare la factory
            //TODO implementare il dictionary
            //TODO eseguire un for per ogni elemento del dictionary

            string input = activityInput.ToLower();
            string sPattern = "(search movie|searchmovie)";
            if (System.Text.RegularExpressions.Regex.IsMatch(input, sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                string pattern = "(search movie |/searchmovie)";
                string replacement = string.Empty;
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(input, replacement);
                this.replyManager = ReplyManagerFactory.genererateReplyManager(this.activity, result, ManagerEnum.SearchMovie);
                return true;
            }
            else
            {
                return false;
            }
            //Activity reply = activity.CreateReply("Sorry, I am only an alpha prototype. So I reply only to the command \"search movie Title\"");

        }


        public override async Task<Activity> computeParsing()
        {
            Activity reply = await this.replyManager.getResponse();  
            return reply;
        }
    }
}