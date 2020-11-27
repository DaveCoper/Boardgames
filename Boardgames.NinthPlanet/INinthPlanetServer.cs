using System.Threading.Tasks;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    public interface INinthPlanetServer
    {
        Task<GameState> GetGameStateAsync();

        Task PlayCardAsync(Card card);

        Task DisplayCardAsync(Card card, TokenPosition? tokenPosition);

        Task TakeGoalAsync(Goal goal);

        Task CallForHelpAsync();
    }
}