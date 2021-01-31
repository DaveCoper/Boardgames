using Boardgames.Client.ViewModels.NinthPlanet;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Client.Factories
{
    public interface INinthPlanetScreenViewModelFactory
    {
        NinthPlanetScreenViewModel CreateInstance(int gameId);

        NinthPlanetScreenViewModel CreateInstance(int gameId, GameInfo gameInfo, GameState state);
    }
}