using System;
using System.Collections.Generic;
using System.Text;
using Boardgames.Game.Models;

namespace Boardgames.NinthPlanet.Models
{
    public class GameState
    {
        public int GameId { get; set; }
        
        public BoardState BoardState { get; set; }

        public LobbyState LobbyState { get; set; }
    }
}
