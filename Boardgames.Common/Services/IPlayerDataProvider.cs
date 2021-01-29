using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Common.Models;

namespace Boardgames.Common.Services
{
    public interface IPlayerDataProvider
    {
        Task<List<PlayerData>> GetPlayerDataAsync(IEnumerable<int> playerIds);

        Task<PlayerData> GetPlayerDataAsync(int playerId);

        Task<PlayerData> GetPlayerDataForCurrentUserAsync();
    }
}