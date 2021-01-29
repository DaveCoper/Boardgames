using System.Collections.Generic;

namespace Boardgames.NinthPlanet.Models
{
    public class LobbyState
    {
        public List<int> ConnectedPlayers { get; set; } = new List<int>();

        public int SelectedLevel { get; set; }
    }
}