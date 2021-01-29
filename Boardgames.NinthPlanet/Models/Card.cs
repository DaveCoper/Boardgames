using System;

namespace Boardgames.NinthPlanet.Models
{
    public struct Card
    {
        public Card(CardColor color, int value)
        {
            Color = color;
            Value = value;
        }

        public CardColor Color { get; set; }

        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Card card &&
                   Color == card.Color &&
                   Value == card.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Value);
        }

        public static bool operator ==(Card c1, Card c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Card c1, Card c2)
        {
            return !c1.Equals(c2);
        }
    }
}