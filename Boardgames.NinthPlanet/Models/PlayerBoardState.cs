using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class PlayerBoardState
    {
        public int NumberOfCardsInHand { get; set; }

        public List<Card> TakenCards { get; set; } = new List<Card>();

        public List<Goal> UnfinishedGoals { get; set; } = new List<Goal>();

        public List<Goal> FinishedGoals { get; set; } = new List<Goal>();

        public Card? DisplayedCard { get; set; }

        public TokenPosition? TokenPosition { get; set; }
    }
}