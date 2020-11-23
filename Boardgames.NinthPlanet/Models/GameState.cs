using System;
using System.Collections.Generic;
using System.Text;
using Boardgames.Game.Models;

namespace Boardgames.NinthPlanet.Models
{
    public class GameState
    {
        public GameInfo GameInfo { get; set; }

        public BoardState BoardState { get; set; }
    }
}
