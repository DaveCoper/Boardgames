using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public interface IPlayerDataService
    {
        Task<List<PlayerData>> GetPlayerData(IEnumerable<int> playerIds);

        Task<PlayerData> GetPlayerData(int playerId);

        Task<PlayerData> GetMyDataAsync();
    }
}