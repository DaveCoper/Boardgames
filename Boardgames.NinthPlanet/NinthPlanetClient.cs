using System;
using System.Threading.Tasks;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.Common.Observables;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet.Client;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet
{
    public class NinthPlanetClient : ObservableObject
    {
        private readonly IPlayerDataProvider playerDataProvider;

        private readonly ILogger<NinthPlanetClient> logger;

        private int concurencyStamp;

        private GameRound currentRound;

        public NinthPlanetClient(
            GameInfo gameInfo,
            IPlayerDataProvider playerDataProvider,
            ILogger<NinthPlanetClient> logger)
        {
            this.GameInfo = gameInfo;
            this.playerDataProvider = playerDataProvider ?? throw new System.ArgumentNullException(nameof(playerDataProvider));
            this.logger = logger ?? NullLogger<NinthPlanetClient>.Instance;
            this.GameLobby = new GameLobby();
            this.concurencyStamp = int.MinValue;
        }

        public GameLobby GameLobby { get; }

        public GameRound CurrentRound
        {
            get => currentRound;
            private set => Set(ref currentRound, value);
        }

        public GameInfo GameInfo { get; }

        public int GameId => GameInfo.Id;

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
                this.CurrentRound = null;
            }
            else
            {
                var round = new GameRound(this.playerDataProvider);
                await round.UpdateState(gameState.RoundState);
                this.CurrentRound = round;
            }

            var players = await playerDataProvider.GetPlayerDataAsync(gameState.LobbyState.ConnectedPlayers);
            var currentPlayer = await playerDataProvider.GetPlayerDataForCurrentUserAsync();
            this.GameLobby.ConnectedPlayers = new ObservableList<PlayerData>(players);
            this.GameLobby.CurrentUserIsGameOwner = GameInfo.OwnerId == currentPlayer.Id;
        }

        public async Task ReceiveMessageAsync(CardWasPlayed cardWasPlayed)
        {
            if (!ProcessMessage(cardWasPlayed))
                return;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            await this.CurrentRound.CardWasPlayedAsync(cardWasPlayed.PlayerId, cardWasPlayed.Card);
        }

        public async Task ReceiveMessageAsync(GameHasStarted gameHasStarted)
        {
            if (!ProcessMessage(gameHasStarted))
                return;

            await this.UpdateStateAsync(gameHasStarted.State);
        }

        public async Task ReceiveMessageAsync(NewPlayerConnected newPlayerConnected)
        {
            if (!ProcessMessage(newPlayerConnected))
                return;

            var newPlayerData = await this.playerDataProvider.GetPlayerDataAsync(newPlayerConnected.NewPlayerId);
            this.GameLobby.ConnectedPlayers.Add(newPlayerData);
        }

        public Task ReceiveMessageAsync(SelectedMissionHasChanged selectedMissionHasChanged)
        {
            if (!ProcessMessage(selectedMissionHasChanged))
                return Task.CompletedTask;

            if (this.CurrentRound != null)
                throw new InvalidOperationException("You can't change mission in the middle of round.");

            this.GameLobby.SelectedMission = selectedMissionHasChanged.SelectedMission;
            return Task.CompletedTask;
        }

        public Task ReceiveMessageAsync(TrickFinished trickFinished)
        {
            if (!ProcessMessage(trickFinished))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound.TrickWasFinished(trickFinished);

            return Task.CompletedTask;
        }

        public Task ReceiveMessageAsync(TaskWasTaken taskWasTaken)
        {
            if (!ProcessMessage(taskWasTaken))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound.TaskWasTaken(taskWasTaken.PlayerId, taskWasTaken.TaskCard);
            return Task.CompletedTask;
        }

        public Task ReceiveMessageAsync(RoundFailed roundFailed)
        {
            if (!ProcessMessage(roundFailed))
                return Task.CompletedTask;

            if (this.CurrentRound == null)
                throw new InvalidOperationException("Round was not started!");

            this.CurrentRound = null;
            return Task.CompletedTask;
        }

        public Task ReceiveMessageAsync(PlayerHasLeft playerHasLeft)
        {
            if (!ProcessMessage(playerHasLeft))
                return Task.CompletedTask;

            throw new NotImplementedException();
        }

        public Task ReceiveMessageAsync(PlayerCommunicatedCard playerCommunicatedCard)
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