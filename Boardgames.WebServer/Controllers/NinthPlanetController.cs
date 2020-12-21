using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Models;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using Boardgames.WebServer.Hubs;
using Boardgames.WebServer.Messages;
using Boardgames.WebServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Boardgames.WebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NinthPlanetController : ControllerBase
    {
        private readonly INinthPlanetGameRepository gameRepository;

        private readonly GameMessenger gameMessenger;

        public NinthPlanetController(
            INinthPlanetGameRepository gameRepository,
            IHubContext<GameHub> hubContext)
        {
            this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            this.gameMessenger = new GameMessenger(GameType.NinthPlanet, hubContext, null);
        }

        [HttpPost("Create")]
        public async Task<GameState> CreateGameAsync(
            [FromBody] NinthPlanetNewGameOptions gameOptions,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();

            var game = await gameRepository.CreateNewGameAsync(userId, gameOptions, cancellationToken);
            return await game.GetGameStateAsync(userId);
        }

        [HttpGet("{gameId}")]
        public async Task<GameState> GetGameStateAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            return await game.GetGameStateAsync(userId);
        }

        [HttpGet("{gameId}/Join")]
        public async Task<GameState> JoinGameAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var gameState = await game.JoinGameAsync(userId, gameMessenger);
            await gameMessenger.FlushAsync();

            return gameState;
        }

        [HttpPost("{gameId}/Leave")]
        public async Task LeaveGameAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await game.LeaveGameAsync(userId, this.gameMessenger);
            await this.gameMessenger.FlushAsync();
        }

        [HttpGet("{gameId}/BeginRound")]
        public async Task BeginRoundAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await game.BeginRoundAsync(userId, this.gameMessenger);
            await this.gameMessenger.FlushAsync();
        }

        [HttpPost("{gameId}/Help")]
        public async Task CallForHelpAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            
            await game.CallForHelpAsync(userId, this.gameMessenger);
            await this.gameMessenger.FlushAsync();
        }

        [HttpPost("{gameId}/Display")]
        public async Task DisplayCardAsync(
            [FromRoute] int gameId,
            [FromBody] CardDisplayInfo input,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await game.DisplayCardAsync(userId, input.Card, input.TokenPosition, this.gameMessenger);
            await this.gameMessenger.FlushAsync();
        }

        [HttpPost("{gameId}/Play")]
        public async Task PlayCardAsync(
            [FromRoute] int gameId,
            [FromBody] Card card,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await game.PlayCardAsync(userId, card, this.gameMessenger);
            await this.gameMessenger.FlushAsync();
        }

        [HttpPost("{gameId}/TakeGoal")]
        public async Task TakeGoalAsync(
            [FromRoute] int gameId,
            [FromBody] TaskCard goal,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            await game.TakeGoalAsync(userId, goal, this.gameMessenger);
            await this.gameMessenger.FlushAsync();
        }

        private int GetUserId()
        {
            //var claimUserId = userManager.GetUserId(this.User);
            var claimUserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claimUserId, out int userId))
            {
                throw new NotImplementedException("Something is wrong with authentication");
            }

            return userId;
        }
    }
}