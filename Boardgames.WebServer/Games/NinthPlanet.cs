using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.WebServer.Games
{
    public class NinthPlanet : INinthPlanetServer
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

        public async Task CallForHelpAsync(int playerId, Queue<GameMessage> messageQueue)
        {
            await semaphore.WaitAsync();
            try
            {
                await server.CallForHelpAsync(playerId, messageQueue);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task DisplayCardAsync(
            int playerId,
            Card card,
            ComunicationTokenPosition? tokenPosition,
            Queue<GameMessage> messageQueue)
        {
            await semaphore.WaitAsync();
            try
            {
                await server.DisplayCardAsync(
                    playerId,
                    card,
                    tokenPosition,
                    messageQueue);
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
                return await server.GetGameStateAsync(playerId);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<GameState> JoinGameAsync(
            int playerId,
            Queue<GameMessage> messageQueue)
        {
            await semaphore.WaitAsync();
            try
            {
                return await server.JoinGameAsync(playerId, messageQueue);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task LeaveGameAsync(
            int playerId,
            Queue<GameMessage> messageQueue)
        {
            await semaphore.WaitAsync();
            try
            {
                await server.LeaveGameAsync(playerId, messageQueue);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task PlayCardAsync(
            int playerId,
            Card card,
            Queue<GameMessage> messageQueue)
        {
            await semaphore.WaitAsync();
            try
            {
                await server.PlayCardAsync(
                    playerId,
                    card,
                    messageQueue);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task TakeGoalAsync(
            int playerId,
            TaskCard goal,
            Queue<GameMessage> messageQueue)
        {
            await semaphore.WaitAsync();
            try
            {
                await server.TakeGoalAsync(
                    playerId,
                    goal,
                    messageQueue);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}