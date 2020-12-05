using System.Threading.Tasks;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Client.Services
{
    public interface INinthPlanetService
    {
        Task<GameState> GetGameStateAsync(int gameId);

        Task<GameState> JoinGameAsync(int gameId);

        Task PlayCardAsync(int gameId, Card card);

        Task DisplayCardAsync(int gameId, Card card, ComunicationTokenPosition? tokenPosition);

        Task TakeGoalAsync(int gameId, TaskCard goal);

        Task CallForHelpAsync(int gameId);
    }
}