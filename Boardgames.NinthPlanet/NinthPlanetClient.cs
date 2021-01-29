using System;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet.Client;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet
{
    public class NinthPlanetClient
    {
        private readonly IPlayerDataProvider playerDataProvider;

        private readonly ILogger<NinthPlanetClient> logger;

        private int concurencyStamp;

        public NinthPlanetClient(int gameId, IPlayerDataProvider playerDataProvider, ILogger<NinthPlanetClient> logger)
        {
            this.GameId = gameId;
            this.playerDataProvider = playerDataProvider ?? throw new System.ArgumentNullException(nameof(playerDataProvider));
            this.logger = logger ?? NullLogger<NinthPlanetClient>.Instance;
            this.GameLobby = new GameLobby();
            this.concurencyStamp = int.MinValue;
        }

        public GameLobby GameLobby { get; }

        public GameRound CurrentRound { get; private set; }

        public int GameId { get; }

        public async Task UpdateStateAsync(GameState gameState)
        {
            if (gameState.GameId != this.GameId)
            {
                throw new ArgumentException("Game state belongs to different game.", nameof(gameState));
            }

            if (this.concurencyStamp > gameState.ConcurencyStamp)
            {
                throw new ArgumentException("Provided game state is older that current game state.", nameof(gameState));
            }

            this.concurencyStamp = gameState.ConcurencyStamp;
            if (gameState.RoundState == null)
            {
                CurrentRound = null;
            }
            else
            {
                CurrentRound = new GameRound(this.playerDataProvider);
                await CurrentRound.UpdateState(gameState.RoundState);
            }

            this.GameLobby.ConnectedPlayers = await playerDataProvider.GetPlayerDataAsync(gameState.LobbyState.ConnectedPlayers);
        }

        public async Task RouteMessage(IGameMessage gameMessage)
        {
            switch (gameMessage)
            {
                case CardWasPlayed cardWasPlayed:
                    await this.ReceiveMessage(cardWasPlayed);
                    break;

                case SelectedMissionHasChanged selectedMissionHasChanged:
                    await this.ReceiveMessage(selectedMissionHasChanged);
                    break;

                case GameHasStarted gameHasStarted:
                    await this.ReceiveMessage(gameHasStarted);
                    break;

                case NewPlayerConnected newPlayerConnected:
                    await ReceiveMessage(newPlayerConnected);
                    break;

                case PlayerCommunicatedCard playerCommunicatedCard:
                    await ReceiveMessage(playerCommunicatedCard);
                    break;

                case PlayerHasLeft playerHasLeft:
                    await ReceiveMessage(playerHasLeft);
                    break;

                case RoundFailed roundFailed:
                    await ReceiveMessage(roundFailed);
                    break;

                case TaskWasTaken taskWasTaken:
                    await ReceiveMessage(taskWasTaken);
                    break;

                case TrickFinished trickFinished:
                    await ReceiveMessage(trickFinished);
                    break;
            }
        }

        public Task ReceiveMessage(CardWasPlayed cardWasPlayed)
        {
            if (!ProcessMessage(cardWasPlayed))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound.CardWasPlayed(cardWasPlayed.PlayerId, cardWasPlayed.Card);
            return Task.CompletedTask;
        }

        public async Task ReceiveMessage(GameHasStarted gameHasStarted)
        {
            if (!ProcessMessage(gameHasStarted))
                return;

            await this.UpdateStateAsync(gameHasStarted.State);
        }

        public async Task ReceiveMessage(NewPlayerConnected newPlayerConnected)
        {
            if (!ProcessMessage(newPlayerConnected))
                return;

            var newPlayerData = await this.playerDataProvider.GetPlayerDataAsync(newPlayerConnected.NewPlayerId);
            this.GameLobby.ConnectedPlayers.Add(newPlayerData);
        }

        private Task ReceiveMessage(SelectedMissionHasChanged selectedMissionHasChanged)
        {
            if (!ProcessMessage(selectedMissionHasChanged))
                return Task.CompletedTask;

            if (this.CurrentRound != null)
                throw new InvalidOperationException("You can't change mission in the middle of round.");

            this.GameLobby.SelectedMission = selectedMissionHasChanged.SelectedMission;
            return Task.CompletedTask;
        }

        private Task ReceiveMessage(TrickFinished trickFinished)
        {
            if (!ProcessMessage(trickFinished))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound.TrickWasFinished(trickFinished);

            return Task.CompletedTask;
        }

        private Task ReceiveMessage(TaskWasTaken taskWasTaken)
        {
            if (!ProcessMessage(taskWasTaken))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound.TrickWasTaken(taskWasTaken.PlayerId, taskWasTaken.TaskCard);
            return Task.CompletedTask;
        }

        private Task ReceiveMessage(RoundFailed roundFailed)
        {
            if (!ProcessMessage(roundFailed))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound = null;
            return Task.CompletedTask;
        }

        private Task ReceiveMessage(PlayerHasLeft playerHasLeft)
        {
            if (!ProcessMessage(playerHasLeft))
                return Task.CompletedTask;

            throw new NotImplementedException();
        }

        private Task ReceiveMessage(PlayerCommunicatedCard playerCommunicatedCard)
        {
            if (!ProcessMessage(playerCommunicatedCard))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound.PlayerComunicatedCard(
                playerCommunicatedCard.PlayerId,
                playerCommunicatedCard.Card,
                playerCommunicatedCard.TokenPosition);

            return Task.CompletedTask;
        }

        private bool ProcessMessage(IGameMessage gameMessage)
        {
            if (gameMessage.GameId != this.GameId)
            {
                throw new ArgumentException("Message belongs to different game.", nameof(gameMessage));
            }

            if (gameMessage.ConcurencyStamp < this.concurencyStamp)
            {
                // TODO maybe throw exception that desync occurred so we know we are supposed to fetch game state again.
                logger.LogInformation("Message was dropped because it was older than current state.");
                return false;
            }

            this.concurencyStamp = gameMessage.ConcurencyStamp;
            return true;
        }
    }
}