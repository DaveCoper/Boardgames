using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public interface IGameInfoService
    {
        Task<List<GameInfo>> GetPublicGamesAsync(CancellationToken cancellationToken = default);
    }
}