using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet.Messages
{
    public class GameHasStarted : GameMessage
    {
        public GameState State { get; set; }
    }
}