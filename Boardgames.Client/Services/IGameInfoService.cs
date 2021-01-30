using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public interface IGameInfoService
    {
        Task<List<GameInfo>> GetPublicGamesAsync(int page = 0, int sizeOfPage = 50, CancellationToken cancellationToken = default);

        Task<int> GetNumberOfPublicGamesAsync(CancellationToken cancellationToken = default);

        Task<GameInfo> GetGameInfoAsync(int gameId, CancellationToken cancellationToken = default);
    }
}