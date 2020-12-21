﻿using System.Threading;
using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.Client.Models;
using Boardgames.WebServer.Data;
using Boardgames.WebServer.Models;
using Boardgames.WebServer.Repositories.Exceptions;
using Boardgames.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Boardgames.WebServer.Repositories
{
    public class NinthPlanetGameRepository : INinthPlanetGameRepository
    {
        private readonly IGameCache<Games.NinthPlanet> gameCache;

        private readonly ApplicationDbContext dbContext;

        private readonly SemaphoreSlim semaphore;

        public NinthPlanetGameRepository(IGameCache<Games.NinthPlanet> gameCache, ApplicationDbContext dbContext)
        {
            semaphore = new SemaphoreSlim(1);

            this.gameCache = gameCache ?? throw new System.ArgumentNullException(nameof(gameCache));
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
        }

        public async Task<Games.NinthPlanet> CreateNewGameAsync(
            int ownerId,
            NinthPlanetNewGameOptions newGameOptions,
            CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var gameInfo = new DbGameInfo
                {
                    GameType = GameType.NinthPlanet,
                    Name = newGameOptions.Name,
                    MaximumNumberOfPlayers = 5,
                    IsPublic = newGameOptions.IsPublic,
                    OwnerId = ownerId
                };

                var state = new NinthPlanetGameState
                {
                    GameInfo = gameInfo
                };

                dbContext.Games.Add(gameInfo);
                dbContext.NinthPlanetGames.Add(state);
                await dbContext.SaveChangesAsync(cancellationToken);

                var game = StartGameFromState(state);
                return game;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<Games.NinthPlanet> GetGameAsync(int gameId, CancellationToken cancellationToken)
        {
            if (this.gameCache.TryGetGame(gameId, out var game))
                return game;

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                // try again, this will handle cases when two or more requests are looking for a same non-cached game
                if (this.gameCache.TryGetGame(gameId, out game))
                    return game;

                var gameState = await dbContext.NinthPlanetGames
                    .Include(x => x.GameInfo)
                    .FirstAsync(x => gameId == x.GameId, cancellationToken);

                if (gameState == null)
                    throw new GameNotFoundException(gameId);

                return StartGameFromState(gameState);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private Games.NinthPlanet StartGameFromState(NinthPlanetGameState gameState)
        {
            var game = new Games.NinthPlanet(new NinthPlanetServer(gameState.GameInfo, gameState));
            this.gameCache.TryAddGame(gameState.GameId, game);
            return game;
        }
    }
}