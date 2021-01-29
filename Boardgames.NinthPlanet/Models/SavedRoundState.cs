using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class SavedRoundState
    {
        public int CaptainPlayerId { get; set; }

        public int CurrentPlayerId { get; set; }

        public CardColor? ColorOfCurrentTrick { get; set; }

        public List<TaskCard> AvailableGoals { get; set; } = new List<TaskCard>();

        public List<int> PlayerOrder { get; set; } = new List<int>();

        public Dictionary<int, PlayerPrivateState> PlayerStates { get; set; } = new Dictionary<int, PlayerPrivateState>();

        public Dictionary<int, Card> CurrentTrick { get; set; } = new Dictionary<int, Card>();

        public bool HelpIsAvailable { get; set; }
    }
}