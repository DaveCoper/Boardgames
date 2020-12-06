using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Boardgames.WebServer.Repositories
{
    public interface IGameRepository<TGameType, TNewGameOptions>
    {
        public Task<TGameType> GetGameAsync(int gameId, CancellationToken cancellationToken);

        public Task<TGameType> CreateNewGameAsync(int ownerId, TNewGameOptions newGameOptions, CancellationToken cancellationToken);
    }
}
