using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.Common.Services;

namespace Boardgames.NinthPlanet.Tests.Utilitites
{
    internal class MemoryPlayerDataProvider : IPlayerDataProvider
    {
        private readonly Dictionary<int, PlayerData> players;

        private readonly int currentPlayerId;

        public MemoryPlayerDataProvider(int currentPlayerId, List<PlayerData> players)
        {
            this.players = players.ToDictionary(x => x.Id);
            this.currentPlayerId = currentPlayerId;
        }

        public Task<List<PlayerData>> GetPlayerDataAsync(IEnumerable<int> playerIds)
        {
            var result = playerIds.Select(x => players[x]).ToList();
            return Task.FromResult(result);
        }

        public Task<PlayerData> GetPlayerDataAsync(int playerId)
        {
            return Task.FromResult(players[playerId]);
        }

        public Task<PlayerData> GetPlayerDataForCurrentUserAsync()
        {
            return GetPlayerDataAsync(this.currentPlayerId);
        }
    }
}