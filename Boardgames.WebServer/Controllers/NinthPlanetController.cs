using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.NinthPlanet.Models;
using Boardgames.Client.Models;
using Boardgames.WebServer.Models;
using Boardgames.WebServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Boardgames.WebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NinthPlanetController : ControllerBase
    {
        private readonly INinthPlanetGameRepository gameRepository;

        private readonly UserManager<ApplicationUser> userManager;

        public NinthPlanetController(
            INinthPlanetGameRepository gameRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpPost("Create")]
        public async Task<GameState> CreateGameAsync(
            [FromBody] NinthPlanetNewGameOptions gameOptions,
            CancellationToken cancellationToken)
        {
            //var claimUserId = userManager.GetUserId(this.User);
            var claimUserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claimUserId, out int userId))
            {
                throw new NotImplementedException("Something is wrong with authentication");
            }

            var game = await gameRepository.StartNewGameAsync(userId, gameOptions, cancellationToken);
            return await game.GetGameStateAsync();
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