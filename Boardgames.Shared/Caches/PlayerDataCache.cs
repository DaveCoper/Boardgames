using System.Collections.Concurrent;
using Boardgames.Shared.Models;

namespace Boardgames.Shared.Caches
{
    public class PlayerDataCache : IPlayerDataCache
    {
        private ConcurrentDictionary<int, PlayerData> cache = new ConcurrentDictionary<int, PlayerData>();

        public bool TryAddValue(int playerId, PlayerData playerData)
        {
            return cache.TryAdd(playerId, playerData);
        }

        public bool TryGetValue(int playerId, out PlayerData playerData)
        {
            return cache.TryGetValue(playerId, out playerData);
        }
    }
}