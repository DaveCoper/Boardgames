using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    public interface INinthPlanetServer
    {
        int GameId { get; }

        int GameOwnerId { get; }

        Task<GameState> GetGameStateAsync(int playerId);

        Task PlayCardAsync(int playerId, Card card, Queue<GameMessage> messageQueue);

        Task<GameState> JoinGameAsync(int newPlayerId, Queue<GameMessage> messageQueue);

        Task LeaveGameAsync(int playerId, Queue<GameMessage> messageQueue);

        Task DisplayCardAsync(int playerId, Card card, ComunicationTokenPosition? tokenPosition, Queue<GameMessage> messageQueue);

        Task TakeGoalAsync(int playerId, TaskCard goal, Queue<GameMessage> messageQueue);

        Task CallForHelpAsync(int playerId, Queue<GameMessage> messageQueue);
    }
}