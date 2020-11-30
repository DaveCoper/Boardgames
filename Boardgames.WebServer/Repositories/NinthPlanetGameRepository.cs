using System.Threading;
using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.Client.Models;
using Boardgames.WebServer.Data;
using Boardgames.WebServer.Models;
using Boardgames.WebServer.Repositories.Exceptions;
using Boardgames.Common.Models;

namespace Boardgames.WebServer.Repositories
{
    public class NinthPlanetGameRepository : INinthPlanetGameRepository
    {
        private readonly IGameCache<INinthPlanetServer> gameCache;

        private readonly ApplicationDbContext dbContext;

        private readonly SemaphoreSlim semaphore;

        public NinthPlanetGameRepository(IGameCache<INinthPlanetServer> gameCache, ApplicationDbContext dbContext)
        {
            semaphore = new SemaphoreSlim(1);

            this.gameCache = gameCache ?? throw new System.ArgumentNullException(nameof(gameCache));
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
        }

        public async Task<INinthPlanetServer> StartNewGameAsync(
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

        public async Task<INinthPlanetServer> GetGameAsync(int gameId, CancellationToken cancellationToken)
        {
            if (this.gameCache.TryGetGame(gameId, out var game))
                return game;

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                // try again, this will handle cases when two or more requests are looking for a same non-cached game
                if (this.gameCache.TryGetGame(gameId, out game))
                    return game;

                var gameState = await dbContext.NinthPlanetGames.FindAsync(
                    new object[] { gameId }, 
                    cancellationToken);

                if (gameState == null)
                    throw new GameNotFoundException(gameId);

                return StartGameFromState(gameState);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private INinthPlanetServer StartGameFromState(NinthPlanetGameState gameState)
        {
            var game = new Games.NinthPlanet(gameState.GameInfo.Id, gameState);
            this.gameCache.TryAddGame(gameState.GameId, game);
            return game;
        }
    }
}