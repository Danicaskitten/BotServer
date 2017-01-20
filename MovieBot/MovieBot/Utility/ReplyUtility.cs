using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Utility
{
    /// <summary>
    /// This class contains useful methods used by <see cref="MovieBot.States.SearchState"/>
    /// </summary>
    public static class ReplyUtility
    {
        public static HeroCard generateHeroCardStateReply(List<CardAction> cardButtons, string heroCardTitle, string heroCardSub)
        {
            HeroCard plCard = new HeroCard()
            {
                Title = heroCardTitle,
                Subtitle = heroCardSub,
                Buttons = cardButtons
            };
            return plCard;
        }
        
        /// <summary>
        /// This method generates the <see cref="StateReply"/> with the <see cref="HeroCard"/> containg all the possible day that
        /// the user can select. It takes the current day and generate all the day until the next Tuesday
        /// </summary>
        /// <param name="replyMessage"></param>
        /// <returns></returns>
        public static StateReply generateWeekDayReply(string replyMessage)
        {
            StateReply reply = new StateReply(false, replyMessage, "herocard");
            string heroCardTitle = "These are the possible dates";

            List<CardAction> cardButtons = new List<CardAction>();
            DateTime today = DateTime.Today;
            int daysUntilTuesday = (((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7) + 1;
            for (int i = 0; i < daysUntilTuesday; i++)
            {
                DateTime day = today.AddDays(i);
                string title = day.DayOfWeek.ToString() + " " +day.ToString("MM/dd/yyyy") ;
                string value = generateValueReplyForHeroCard(ValueEnum.Day,false)+day.ToString("MM/dd/yyyy");
                CardAction plButton = new CardAction()
                {
                    Value = value,
                    Type = "imBack",
                    Title = title
                };
                cardButtons.Add(plButton);
            }
            reply.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "Please select one option");
            return reply;
        }

        /// <summary>
        /// This Method returns the right Value field to be added in the heroCard
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="toSubstitue">True if the return value will be using in the parsing, false otherwise</param>
        /// <returns></returns>
        public static string generateValueReplyForHeroCard(ValueEnum valueType, bool toSubstitue)
        {
            string valueToReturn="";
            switch (valueType)
            {
                case ValueEnum.Day:
                    valueToReturn = "You selected: ";
                    break;
                case ValueEnum.Cinema:
                    valueToReturn = "Cinema selected: ";
                    break;
                case ValueEnum.Movie:
                    valueToReturn = "Movie selected: ";
                    break;
                case ValueEnum.Location:
                    valueToReturn = "City selected: ";
                    break;
                case ValueEnum.Projections:
                    valueToReturn = "You selected the projection number: ";
                    break;
            }
            if (toSubstitue)
            {
                return valueToReturn.ToLower();
            }
            else
            {
                return valueToReturn;
            }
        }
    }
}