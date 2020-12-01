using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public interface IPlayerDataService
    {
        Task<List<PlayerData>> GetPlayerDataAsync(IEnumerable<int> playerIds);

        Task<PlayerData> GetPlayerDataAsync(int playerId);

        Task<PlayerData> GetMyDataAsync();
    }
}