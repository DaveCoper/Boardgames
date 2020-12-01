using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public class GameInfoService : IGameInfoService
    {
        private readonly IWebApiBrooker webApiBrooker;

        public GameInfoService(IWebApiBrooker webApiBrooker)
        {
            this.webApiBrooker = webApiBrooker ?? throw new System.ArgumentNullException(nameof(webApiBrooker));
        }

        public async Task<List<GameInfo>> GetPublicGamesAsync(CancellationToken cancellationToken = default)
        {
            return await this.webApiBrooker.GetAsync<List<GameInfo>>("Game", cancellationToken: cancellationToken);
        }
    }
}