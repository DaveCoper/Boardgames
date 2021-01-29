using System.Collections.Generic;
using Boardgames.Common.Models;

namespace Boardgames.NinthPlanet.Client
{
    public class GameLobby
    {
        public List<PlayerData> ConnectedPlayers { get; set; }

        public int SelectedMission { get; set; }
    }
}
