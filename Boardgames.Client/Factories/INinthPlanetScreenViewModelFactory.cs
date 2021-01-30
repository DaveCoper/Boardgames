using Boardgames.Client.ViewModels.NinthPlanet;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Client.Factories
{
    public interface INinthPlanetScreenViewModelFactory
    {
        NinthPlanetScreenViewModel CreateInstance(int ownerId, int gameId, GameState state);
    }
}