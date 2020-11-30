using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class GameHasStarted
    {
        public GameState State { get; set; }
    }
}