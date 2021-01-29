using System.Collections.Generic;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.WebServer.Models
{
    public class NinthPlanetGameState
    {
        public int GameId { get; set; }

        public DbGameInfo GameInfo { get; set; }

        public ICollection<NinthPlanetPlayerState> PlayerStates { get; set; }

        public bool RoundIsInProgress { get; set; }
        public int CurrentPlayerId { get; set; }
        public int CaptainPlayerId { get; internal set; }
        public CardColor? ColorOfCurrentTrick { get; internal set; }
    }
}