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

        GameState GetGameState(int playerId);
        
        SavedGameState SaveCurrentState();

        void PlayCard(int playerId, Card card, IGameMessenger gameMessenger);

        GameState JoinGame(int newPlayerId, IGameMessenger gameMessenger);

        void LeaveGame(int playerId, IGameMessenger gameMessenger);

        void DisplayCard(int playerId, Card card, CommunicationTokenPosition? tokenPosition, IGameMessenger gameMessenger);

        void TakeTaskCard(int playerId, TaskCard goal, IGameMessenger gameMessenger);

        void CallForHelp(int playerId, IGameMessenger gameMessenger);
        
        void BeginRound(int userId, IGameMessenger gameMessenger);
    }
}