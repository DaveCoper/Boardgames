using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.Web.Server.Repositories
{
    public interface IGameRepository<TGameType>
    {
        public Task<TGameType> GetGameAsync(int gameId, CancellationToken cancellationToken);

        public Task<TGameType> CreateGameAsync(CancellationToken cancellationToken);
    }
}
