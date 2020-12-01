using System;
using System.Globalization;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.Client.Services
{
    public class NinthPlanetService : INinthPlanetService
    {
        private const string ControllerName = "NinthPlanet";

        private readonly IWebApiBrooker webApiBrooker;

        public NinthPlanetService(IWebApiBrooker webApiBrooker)
        {
            this.webApiBrooker = webApiBrooker ?? throw new ArgumentNullException(nameof(webApiBrooker));
        }

        public Task CallForHelpAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task DisplayCardAsync(int gameId, Card card, TokenPosition? tokenPosition)
        {
            throw new NotImplementedException();
        }

        public async Task<GameState> GetGameStateAsync(int gameId)
        {
            return await this.webApiBrooker.GetAsync<GameState>(ControllerName, gameId.ToString(CultureInfo.InvariantCulture));
        }

        public Task PlayCardAsync(int gameId, Card card)
        {
            throw new NotImplementedException();
        }

        public Task TakeGoalAsync(int gameId, Goal goal)
        {
            throw new NotImplementedException();
        }
    }
}