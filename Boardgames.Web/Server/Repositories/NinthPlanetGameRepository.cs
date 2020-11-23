using System;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.Web.Server.Data;
using Boardgames.Web.Server.Repositories.Exceptions;

namespace Boardgames.Web.Server.Repositories
{
    public class NinthPlanetGameRepository : IGameRepository<NinthPlanet.IServer>
    {
        private readonly IGameCache<NinthPlanet.IServer> gameCache;
        private readonly ApplicationDbContext dbContext;
        private readonly SemaphoreSlim semaphore;

        public NinthPlanetGameRepository(IGameCache<NinthPlanet.IServer> gameCache, ApplicationDbContext dbContext)
        {
            semaphore = new SemaphoreSlim(1);

            this.gameCache = gameCache ?? throw new System.ArgumentNullException(nameof(gameCache));
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
        }

        public async Task<IServer> CreateGameAsync(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync();
            try
            {
                throw new NotImplementedException();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<IServer> GetGameAsync(int gameId, CancellationToken cancellationToken)
        {
            if (this.gameCache.TryGetGame(gameId, out var game))
                return game;

            await semaphore.WaitAsync();
            try
            {
                // try again, this will handle cases when two or more requests are looking for a same non-cached game
                if (this.gameCache.TryGetGame(gameId, out game))
                    return game;

                var gameState = dbContext.NinthPlanetGames.Find(gameId);
                if (gameState == null)
                    throw new GameNotFoundException(gameId);

                game = new Games.NinthPlanet(gameState);
                this.gameCache.TryAddGame(gameId, game);
                return game;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}