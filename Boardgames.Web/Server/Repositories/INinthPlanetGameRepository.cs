using Boardgames.NinthPlanet;
using Boardgames.Shared.Models;

namespace Boardgames.Web.Server.Repositories
{
    public interface INinthPlanetGameRepository : IGameRepository<INinthPlanetServer, NinthPlanetNewGameOptions>
    {
    }
}