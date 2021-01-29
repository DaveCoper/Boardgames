using System.Threading;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.WebServer.Games
{
    public class NinthPlanet
    {
        private readonly SemaphoreSlim semaphore;

        private readonly INinthPlanetServer server;

        public NinthPlanet(INinthPlanetServer server)
        {
            this.semaphore = new SemaphoreSlim(1);
            this.server = server;
        }

        public int GameId => server.GameId;

        public int GameOwnerId => server.GameOwnerId;

        public async Task BeginRoundAsync(int playerId, IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                server.BeginRound(playerId, gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task CallForHelpAsync(int playerId, IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                server.CallForHelp(playerId, gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task DisplayCardAsync(
            int playerId,
            Card? card,
            CommunicationTokenPosition? tokenPosition,
            IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                server.DisplayCard(
                    playerId,
                    card,
                    tokenPosition,
                    gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<GameState> GetGameStateAsync(int playerId)
        {
            await semaphore.WaitAsync();
            try
            {
                return server.GetGameState(playerId);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<GameState> JoinGameAsync(
            int playerId,
            IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                return server.JoinGame(playerId, gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task LeaveGameAsync(
            int playerId,
            IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                server.LeaveGame(playerId, gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task PlayCardAsync(
            int playerId,
            Card card,
            IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                server.PlayCard(
                    playerId,
                    card,
                    gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task TakeGoalAsync(
            int playerId,
            TaskCard goal,
            IGameMessenger gameMessenger)
        {
            await semaphore.WaitAsync();
            try
            {
                server.TakeTaskCard(
                    playerId,
                    goal,
                    gameMessenger);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<SavedGameState> SaveCurrentStateAsync()
        {
            await semaphore.WaitAsync();
            try
            {
                return server.SaveCurrentState();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}