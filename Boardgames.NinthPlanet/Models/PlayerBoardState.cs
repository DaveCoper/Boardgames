using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class PlayerBoardState
    {
        public int NumberOfCardsInHand { get; set; }

        public List<Card> TakenCards { get; set; } = new List<Card>();

        public List<TaskCard> UnfinishedGoals { get; set; } = new List<TaskCard>();

        public List<TaskCard> FinishedGoals { get; set; } = new List<TaskCard>();

        public Card? ComunicatedCard { get; set; }

        public CommunicationTokenPosition? CommunicationTokenPosition { get; set; }
    }
}