using System;
using System.Collections.Generic;
using System.Text;

namespace Boardgames.NinthPlanet.Models
{

    public struct Goal
    {
        public Card Card { get; set; }

        public GoalOrder? Token { get; set; }
    }
}
