using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class PlayerPrivateState
    {
        public List<Card> CardsInHand { get; set; } = new List<Card>();

        public List<TaskCard> TasksCards { get; set; } = new List<TaskCard>();

        public Card? ComunicatedCard { get; set; }

        public ComunicationTokenPosition? ComunicationTokenPosition { get; set; }
    }
}