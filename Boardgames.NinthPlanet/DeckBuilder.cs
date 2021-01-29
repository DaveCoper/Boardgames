using System.Collections.Generic;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    public static class DeckBuilder
    {
        public static List<Card> CreateCardDeck()
        {
            var deck = CreateGoalDeck();

            // add rocket cards
            for (int i = 1; i <= 4; ++i)
            {
                deck.Add(new Card { Value = i, Color = CardColor.Rocket });
            }

            return deck;
        }

        public static List<Card> CreateGoalDeck()
        {
            // 9 cards 4 colors
            List<Card> deck = new List<Card>(9 * 4);
            for (int i = 1; i <= 9; ++i)
            {
                deck.Add(new Card { Value = i, Color = CardColor.Blue });
                deck.Add(new Card { Value = i, Color = CardColor.Green });
                deck.Add(new Card { Value = i, Color = CardColor.Pink });
                deck.Add(new Card { Value = i, Color = CardColor.Yellow });
            }

            return deck;
        }
    }
}