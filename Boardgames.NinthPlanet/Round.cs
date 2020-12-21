using System.Collections.Generic;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    internal class Round
    {
        public Dictionary<int, PlayerPrivateState> StateOfPlayersAtTable { get; set; }

        public List<TaskCard> AvailableGoals { get; set; }

        public List<int> PlayerOrder { get; set; }

        public int CurrentPlayerId { get; set; }

        public CardColor? ColorOfCurrentTrick { get; set; }

        public int CurrentCaptainPlayerId { get; set; }

        public Dictionary<int, Card> CurrentTrick { get; internal set; }
    }
}