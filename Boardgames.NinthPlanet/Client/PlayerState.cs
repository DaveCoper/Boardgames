using System.Collections.Generic;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Client
{
    public class PlayerState
    {
        public Card? DisplayedCard { get; set; }

        public CommunicationTokenPosition? CommunicationTokenPosition { get; set; }

        public List<Card> TakenCards { get; set; }

        public PlayerData PlayerData { get; set; }

        public int NumberOfCards { get; set; }

        public List<TaskCard> UnfinishedTasks { get; set; } = new List<TaskCard>();

        public List<TaskCard> FinishedTasks { get; set; } = new List<TaskCard>();
    }
}