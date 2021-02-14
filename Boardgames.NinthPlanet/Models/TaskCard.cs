using System;

namespace Boardgames.NinthPlanet.Models
{
    public class TaskCard
    {
        public Card Card { get; set; }

        public TaskCardModifier? Modifier { get; set; }

        public static bool operator ==(TaskCard c1, TaskCard c2)
        {
            return ReferenceEquals(c1, c2) || (!ReferenceEquals(c1, null) && c1.Equals(c2));
        }

        public static bool operator !=(TaskCard c1, TaskCard c2)
        {
            return !ReferenceEquals(c1, c2) && (ReferenceEquals(c1, null) || !c1.Equals(c2));
        }

        public override bool Equals(object obj)
        {
            return obj is TaskCard card &&
                   Card == card.Card &&
                   Modifier == card.Modifier;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Card, Modifier);
        }

        public override string ToString()
        {
            if (Modifier.HasValue)
            {
                return $"{Modifier.Value} {Card}";
            }

            return Card.ToString();
        }
    }
}