using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Client.Services
{
    public interface INinthPlanetService
    {
        Task<GameState> GetGameStateAsync(int gameId);

        Task PlayCardAsync(int gameId, Card card);

        Task DisplayCardAsync(int gameId, Card card, TokenPosition? tokenPosition);

        Task TakeGoalAsync(int gameId, Goal goal);

        Task CallForHelpAsync(int gameId);
    }
}