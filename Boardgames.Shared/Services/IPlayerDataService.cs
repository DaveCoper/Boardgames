using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Shared.Models;

namespace Boardgames.Shared.Services
{
    public interface IPlayerDataService
    {
        Task<List<PlayerData>> GetPlayerData(IEnumerable<int> playerIds);

        Task<PlayerData> GetPlayerData(int playerId);

        Task<PlayerData> GetMyDataAsync();
    }
}