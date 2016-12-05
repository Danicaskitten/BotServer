using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieBot.Utility
{
    public static class ReplyUtilitycs
    {
        public static HeroCard generatesHeroCardStateReply(List<CardAction> cardButtons, string heroCardTitle, string heroCardSub)
        {
            HeroCard plCard = new HeroCard()
            {
                Title = heroCardTitle,
                Subtitle = heroCardSub,
                Buttons = cardButtons
            };
            return plCard;
        }
    }
}