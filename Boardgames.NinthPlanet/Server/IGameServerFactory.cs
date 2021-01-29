using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using Microsoft.Extensions.Logging;

namespace Boardgames.NinthPlanet.Server
{
    public interface IGameServerFactory
    {
        NinthPlanetServer CreateGameFromState(SavedGameState gameState);

        NinthPlanetServer CreateNewGame(GameInfo gameInfo);
    }
}