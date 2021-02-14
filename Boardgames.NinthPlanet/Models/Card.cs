using System;

namespace Boardgames.NinthPlanet.Models
{
    public class Card
    {
        public Card()
        {
        }

        public Card(CardColor color, int value)
        {
            Color = color;
            Value = value;
        }

        public CardColor Color { get; set; }

        public int Value { get; set; }

        public static bool operator ==(Card c1, Card c2)
        {
            return ReferenceEquals(c1, c2) || (!ReferenceEquals(c1, null) && c1.Equals(c2));
        }

        public static bool operator !=(Card c1, Card c2)
        {
            return !ReferenceEquals(c1, c2) && (ReferenceEquals(c1, null) || !c1.Equals(c2));
        }

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

        public override string ToString()
        {
            return $"{Color} {Value}";
        }
    }
}