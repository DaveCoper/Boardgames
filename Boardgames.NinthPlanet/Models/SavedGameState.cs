using System.Collections.Generic;
using Boardgames.Common.Models;

namespace Boardgames.NinthPlanet.Models
{
    public class SavedGameState
    {
        public GameInfo GameInfo { get; set; }

        public List<int> PlayersInLobby { get; set; }

        public SavedRoundState RoundState { get; set; }
    }
}