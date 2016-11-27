using Microsoft.Bot.Connector;
using MovieBot.ReplayManager;
using MovieBot.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MovieBot.Utility
{
    public class MessageStateParser
    {
        public static async Task<Activity> computeStateParsing(Activity activity)
        {
            //TODO implementare la pate di parsing utilizzando un dizionario del tipo {"search by moovie": "regexpr corrispondente"} così che poi posso chiamare una factory che mi restituisce la classe giusta in base al primo pezzo
            //TODO creare la factory
            //TODO implementare il dictionary
            //TODO eseguire un for per ogni elemento del dictionary

            string input = activity.Text.ToLower();
            Activity reply = null;
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            if (userData.GetProperty<bool>("searchMovie"))
            {
                SearchCinemaState addedUserData ;
                BotState botState = new BotState(stateClient);
                BotData botData = await botState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                addedUserData = botData.GetProperty<SearchCinemaState>("SearchState");
                reply = await SearchMovie.getResponseWithState(activity, input, addedUserData);
            }

            return reply;
        }
    }
}