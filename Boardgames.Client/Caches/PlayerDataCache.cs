using System.Collections.Concurrent;
using Boardgames.Common.Models;

namespace Boardgames.Client.Caches
{
    public class PlayerDataCache : IPlayerDataCache
    {
        private readonly ConcurrentDictionary<int, PlayerData> cache = new ConcurrentDictionary<int, PlayerData>();

        public PlayerData CurrentUserData { get; set; }

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