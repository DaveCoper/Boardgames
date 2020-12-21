using Boardgames.Client.Brookers;
using Boardgames.Client.Models;
using Boardgames.NinthPlanet.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;

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

        public async Task BeginRoundAsync(int gameId)
        {
            await this.webApiBrooker.GetAsync(
                ControllerName,
                $"{gameId}/BeginRound");
        }

        public Task CallForHelpAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public async Task DisplayCardAsync(int gameId, Card? card, CommunicationTokenPosition? tokenPosition)
        {
            await this.webApiBrooker.PostAsync<CardDisplayInfo>(
                ControllerName,
                new CardDisplayInfo { Card = card, TokenPosition = tokenPosition },
                $"{gameId}/Display");
        }

        public async Task<GameState> GetGameStateAsync(int gameId)
        {
            return await this.webApiBrooker.GetAsync<GameState>(
                ControllerName,
                gameId.ToString(CultureInfo.InvariantCulture));
        }

        public async Task<GameState> JoinGameAsync(int gameId)
        {
            return await this.webApiBrooker.GetAsync<GameState>(
                ControllerName,
                $"{gameId}/Join");
        }

        public async Task PlayCardAsync(int gameId, Card card)
        {
            await this.webApiBrooker.PostAsync(
                   ControllerName,
                   card,
                   $"{gameId}/Play");
        }

        public Task TakeGoalAsync(int gameId, TaskCard goal)
        {
            throw new NotImplementedException();
        }
    }
}