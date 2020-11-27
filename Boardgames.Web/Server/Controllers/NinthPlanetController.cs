using System;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;
using Boardgames.Shared.Models;
using Boardgames.Web.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boardgames.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NinthPlanetController
    {
        private readonly IGameRepository<INinthPlanetServer, NinthPlanetNewGameOptions> gameRepository;

        public NinthPlanetController(IGameRepository<INinthPlanetServer, NinthPlanetNewGameOptions> gameRepository)
        {
            this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        [HttpPost("{gameId}/Help")]
        public async Task CallForHelpAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            await game.CallForHelpAsync();
        }

        [HttpPost("{gameId}/Display")]
        public async Task DisplayCardAsync(
            [FromRoute] int gameId,
            [FromBody] DisplayCardInput input,
            CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            await game.DisplayCardAsync(input.Card, input.TokenPosition);
        }

        [HttpGet("{gameId}")]
        public async Task<GameState> GetGameStateAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            return await game.GetGameStateAsync();
        }

        [HttpPost("{gameId}/Play")]
        public async Task PlayCardAsync(
            [FromRoute] int gameId,
            [FromBody] Card card,
            CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            await game.PlayCardAsync(card);
        }

        [HttpPost("{gameId}/TakeGoal")]
        public async Task TakeGoalAsync(
            [FromRoute] int gameId,
            [FromBody] Goal goal,
            CancellationToken cancellationToken)
        {
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            await game.TakeGoalAsync(goal);
        }

        public class DisplayCardInput
        {
            public Card Card { get; set; }

            public TokenPosition? TokenPosition { get; set; }
        }
    }
}