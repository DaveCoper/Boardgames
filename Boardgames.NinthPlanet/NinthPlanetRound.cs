using System.Collections.Generic;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    public class NinthPlanetRound
    {
        public NinthPlanetRound()
        {
        }

        public Dictionary<int, PlayerPrivateState> StateOfPlayersAtTable { get; set; }

        public List<TaskCard> AvailableGoals { get; set; }
    }
}