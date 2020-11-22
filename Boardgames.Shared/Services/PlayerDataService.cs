using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Boardgames.Shared.Caches;
using Boardgames.Shared.Models;

/*
using Newtonsoft.Json;

namespace Boardgames.Shared.Services
{
    public class PlayerDataService : IPlayerDataService
    {
        private const string controllerUrl = "/api/playerData";

        private readonly HttpClient httpClient;
        private readonly IPlayerDataCache cache;

        public PlayerDataService(HttpClient httpClient, IPlayerDataCache cache)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<PlayerData> GetPlayerData(int playerId)
        {
            if (cache.TryGetValue(playerId, out var playerData))
            {
                return playerData;
            }

            using (var result = await httpClient.GetAsync($"{controllerUrl}/{playerId}"))
            {
                result.EnsureSuccessStatusCode();

                var json = await result.Content.ReadAsStringAsync();
                playerData = JsonConvert.DeserializeObject<PlayerData>(json);
                if (cache.TryAddValue(playerId, playerData))
                {
                    return playerData;
                }

                if (cache.TryGetValue(playerId, out playerData))
                {
                    return playerData;
                }
            }

            throw new InvalidOperationException("Failed to load player data.");
        }

        public async Task<List<PlayerData>> GetPlayerData(IEnumerable<int> playerIds)
        {
            var idSet = new HashSet<int>();
            var list = new List<PlayerData>();

            foreach (var id in playerIds)
            {
                if (cache.TryGetValue(id, out var playerData))
                {
                    list.Add(playerData);
                }
                else
                {
                    idSet.Add(id);
                }
            }

            if (idSet.Count > 0)
            {
                var outJson = JsonConvert.SerializeObject(idSet);
                using (var content = new StringContent(outJson, Encoding.UTF8, "application/json"))
                {
                    using (var result = await httpClient.PostAsync($"{controllerUrl}", content))
                    {
                        result.EnsureSuccessStatusCode();
                        var resultJson = await result.Content.ReadAsStringAsync();

                        list.AddRange(JsonConvert.DeserializeObject<List<PlayerData>>(resultJson));
                    }
                }
            }

            return list;
        }
    }
}
*/