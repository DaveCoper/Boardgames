using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Common.Models;

namespace Boardgames.Client.Services
{
    public class GameInfoService : IGameInfoService
    {
        private const string ControllerName = "Games";

        private readonly IWebApiBrooker webApiBrooker;

        public GameInfoService(IWebApiBrooker webApiBrooker)
        {
            this.webApiBrooker = webApiBrooker ?? throw new System.ArgumentNullException(nameof(webApiBrooker));
        }

        public async Task<GameInfo> GetGameInfoAsync(int gameId, CancellationToken cancellationToken = default)
        {
            return await this.webApiBrooker.GetAsync<GameInfo>(
                ControllerName, 
                gameId.ToString(CultureInfo.InvariantCulture), 
                cancellationToken: cancellationToken);
        }

        public Task<int> GetNumberOfPublicGamesAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<GameInfo>> GetPublicGamesAsync(int page = 0, int sizeOfPage = 50, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(page), page.ToString(CultureInfo.InvariantCulture) },
                { nameof(sizeOfPage), sizeOfPage.ToString(CultureInfo.InvariantCulture) },
            };

            return await this.webApiBrooker.GetAsync<List<GameInfo>>(
                ControllerName, 
                parameters: parameters,
                cancellationToken: cancellationToken);
        }
    }
}