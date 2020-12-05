using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Boardgames.Client.Models;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Models;
using Boardgames.WebServer.Hubs;
using Boardgames.WebServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Boardgames.WebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NinthPlanetController : ControllerBase
    {
        private readonly INinthPlanetGameRepository gameRepository;

        private readonly IHubContext<GameHub> hubContext;

        public NinthPlanetController(
            INinthPlanetGameRepository gameRepository,
            IHubContext<GameHub> hubContext)
        {
            this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            this.hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        [HttpPost("Create")]
        public async Task<GameState> CreateGameAsync(
            [FromBody] NinthPlanetNewGameOptions gameOptions,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();

            var game = await gameRepository.StartNewGameAsync(userId, gameOptions, cancellationToken);
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

            var msgQueue = new Queue<GameMessage>();
            var gameState = await game.JoinGameAsync(userId, msgQueue);
            await FlushMessagesAsync(msgQueue);

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

            var msgQueue = new Queue<GameMessage>();
            await game.LeaveGameAsync(userId, msgQueue);
            await FlushMessagesAsync(msgQueue);
        }

        [HttpPost("{gameId}/Help")]
        public async Task CallForHelpAsync(
            [FromRoute] int gameId,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var msgQueue = new Queue<GameMessage>();
            await game.CallForHelpAsync(userId, msgQueue);
            await FlushMessagesAsync(msgQueue);
        }

        [HttpPost("{gameId}/Display")]
        public async Task DisplayCardAsync(
            [FromRoute] int gameId,
            [FromBody] DisplayCardInput input,
            CancellationToken cancellationToken)
        {
            int userId = GetUserId();
            var game = await gameRepository.GetGameAsync(gameId, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var msgQueue = new Queue<GameMessage>();
            await game.DisplayCardAsync(userId, input.Card, input.TokenPosition, msgQueue);
            await FlushMessagesAsync(msgQueue);
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

            var msgQueue = new Queue<GameMessage>();
            await game.PlayCardAsync(userId, card, msgQueue);
            await FlushMessagesAsync(msgQueue);
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

            var msgQueue = new Queue<GameMessage>();
            await game.TakeGoalAsync(userId, goal, msgQueue);
            await FlushMessagesAsync(msgQueue);
        }

        private async Task FlushMessagesAsync(Queue<GameMessage> msgQueue)
        {
            string gamePrefix = "NinthPlanet";
            while (msgQueue.Count > 0)
            {
                var msg = msgQueue.Dequeue();
                var connection = hubContext.Clients.User(
                    msg.ReceiverId.ToString(CultureInfo.InvariantCulture));

                if (connection != null)
                {
                    var payloadType = msg.Payload.GetType().Name;
                    await connection.SendAsync($"{gamePrefix}_{payloadType}", msg.Payload);
                }
            }
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

        public class DisplayCardInput
        {
            public Card Card { get; set; }

            public ComunicationTokenPosition? TokenPosition { get; set; }
        }
    }
}