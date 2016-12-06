using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Utility
{
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

        public static StateReply generateWeekDayReply()
        {
            string replayMessage = "These are all the possible dates in which I have found projections";
            StateReply reply = new StateReply(false, replayMessage, "herocard");
            string heroCardTitle = "Please select the day in which you want go to the cinema";

            List<CardAction> cardButtons = new List<CardAction>();
            DateTime today = DateTime.Today;
            int daysUntilTuesday = (((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7) + 1;
            for (int i = 0; i < daysUntilTuesday; i++)
            {
                DateTime day = today.AddDays(i);
                string title = day.DayOfWeek.ToString() + " " +day.ToString("MM/dd/yyyy") ;
                string value = "selectedDay="+day.ToString("MM/dd/yyyy");
                CardAction plButton = new CardAction()
                {
                    Value = value,
                    Type = "imBack",
                    Title = title
                };
                cardButtons.Add(plButton);
            }
            reply.HeroCard = ReplyUtility.generateHeroCardStateReply(cardButtons, heroCardTitle, "please select one");
            return reply;
        }
    }
}