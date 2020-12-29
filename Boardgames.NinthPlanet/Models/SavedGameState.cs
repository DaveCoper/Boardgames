using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class SavedGameState
    {
        public int GameId { get; set; }

        public int GameOwnerId { get; set; }

        public List<int> PlayersIsLobby { get; set; }

        public Round RoundState { get; set; }
    }
}