using System.Collections.Concurrent;

namespace Boardgames.WebServer.Repositories
{
    public class DefaultGameCache<TGameType> : IGameCache<TGameType>
    {
        private readonly ConcurrentDictionary<int, TGameType> cache;

        public DefaultGameCache()
        {
            this.cache = new ConcurrentDictionary<int, TGameType>();
        }

        public bool TryAddGame(int gameId, TGameType game)
        {
            return this.cache.TryAdd(gameId, game);
        }

        public bool TryGetGame(int gameId, out TGameType game)
        {
            return this.cache.TryGetValue(gameId, out game);
        }
    }
}