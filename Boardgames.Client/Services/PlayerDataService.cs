using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Client.Caches;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public class PlayerDataService : IPlayerDataService
    {
        private const string controllerName = "PlayerData";

        private readonly IWebApiBrooker apiBrooker;

        private readonly IPlayerDataCache cache;

        public PlayerDataService(IWebApiBrooker apiBrooker, IPlayerDataCache cache)
        {
            this.apiBrooker = apiBrooker ?? throw new ArgumentNullException(nameof(apiBrooker));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<PlayerData> GetMyDataAsync()
        {
            if (this.cache.CurrentUserData != null)
            {
                return this.cache.CurrentUserData;
            }

            var playerData = await apiBrooker.GetAsync<PlayerData>(
                controllerName: controllerName);

            if (cache.TryAddValue(playerData.Id, playerData))
            {
                this.cache.CurrentUserData = playerData;
                return playerData;
            }

            if (cache.TryGetValue(playerData.Id, out playerData))
            {
                return playerData;
            }

            throw new InvalidOperationException("Failed to load player data.");
        }

        public async Task<PlayerData> GetPlayerData(int playerId)
        {
            if (cache.TryGetValue(playerId, out var playerData))
            {
                return playerData;
            }

            playerData = await apiBrooker.GetAsync<PlayerData>(
                controllerName: controllerName,
                actionName: playerId.ToString());

            if (cache.TryAddValue(playerId, playerData))
            {
                return playerData;
            }

            if (cache.TryGetValue(playerId, out playerData))
            {
                return playerData;
            }
            throw new InvalidOperationException("Failed to load player data.");
        }

        public async Task<List<PlayerData>> GetPlayerDataAsync(IEnumerable<int> playerIds)
        {
            var idsToLoad = new HashSet<int>();
            var result = new List<PlayerData>();

            foreach (var id in playerIds)
            {
                if (cache.TryGetValue(id, out var playerData))
                {
                    result.Add(playerData);
                }
                else
                {
                    idsToLoad.Add(id);
                }
            }

            if (idsToLoad.Count > 0)
            {
                var data = await this.apiBrooker.PostAsync<List<PlayerData>, IEnumerable<int>>(
                    controllerName: controllerName,
                    content: idsToLoad);

                foreach (var playerData in data)
                {
                    cache.TryAddValue(playerData.Id, playerData);
                }

                result.AddRange(data);
            }

            return result;
        }
    }
}