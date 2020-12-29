namespace Boardgames.NinthPlanet.Models
{
    public struct Card
    {
        public CardColor Color { get; set; }

        public int Value { get; set; }

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