using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Services;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public class NinthPlanetScreenViewModel : ContentViewModel, IAsyncLoad, IGameViewModel
    {
        private readonly int gameOwnerId;

        private readonly int gameId;

        private readonly IPlayerDataProvider playerDataService;

        private readonly INinthPlanetService ninthPlanetService;

        private GameState gameState;

        private LobbyViewModel lobbyViewModel;

        private object currentView;

        [Obsolete("Used by WPF designer", true)]
        public NinthPlanetScreenViewModel() : base(Resources.Strings.PlanetNine_Title)
        {
            lobbyViewModel = new LobbyViewModel();
            this.CurrentView = lobbyViewModel;
        }

        public NinthPlanetScreenViewModel(
            int gameOwnerId,
            int gameId,
            GameState gameState,
            IPlayerDataProvider playerDataService,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger,
            ILogger<NinthPlanetScreenViewModel> logger)
            : base(Resources.Strings.PlanetNine_Title, messenger, logger)
        {
            this.gameOwnerId = gameOwnerId;
            this.gameId = gameId;
            this.gameState = gameState;

            if (gameState != null)
            {
                Debug.Assert(gameState.GameId == gameId, "Game ids are not the same!");
            }

            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.ninthPlanetService = ninthPlanetService ?? throw new ArgumentNullException(nameof(ninthPlanetService));

            messenger.Register<GameHasStarted>(this, OnGameHasStarted);
            messenger.Register<NewPlayerConnected>(this, OnPlayerHasConnected);
            messenger.Register<PlayerHasLeft>(this, OnPlayerHasLeft);
        }

        public object CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }

        public int GameId => gameId;

        protected override async Task LoadDataInternalAsync()
        {
            if (gameState == null)
            {
                gameState = await ninthPlanetService.JoinGameAsync(this.gameId);
            }

            var myData = await this.playerDataService.GetPlayerDataForCurrentUserAsync();
            if (gameState.RoundState != null)
            {
                var playerData = await this.playerDataService.GetPlayerDataAsync(gameState.RoundState.PlayerStates.Keys);
                CurrentView = new RoundViewModel(
                    this.gameId,
                    myData.Id,
                    gameState.RoundState,
                    playerData.ToDictionary(x => x.Id),
                    this.ninthPlanetService,
                    this.MessengerInstance);

                return;
            }

            if (gameState.LobbyState != null)
            {
                var playerData = await this.playerDataService.GetPlayerDataAsync(gameState.LobbyState.ConnectedPlayers);
                lobbyViewModel = new LobbyViewModel(playerData, myData.Id == gameOwnerId, this.BeginRound);
                CurrentView = lobbyViewModel;
                return;
            }
        }

        private async void BeginRound()
        {
            await this.ninthPlanetService.BeginRoundAsync(this.gameId);
        }

        private void OnPlayerHasLeft(PlayerHasLeft msg)
        {
            if (msg.GameId != this.gameState.GameId)
                return;

            if (this.lobbyViewModel == null)
            {
                Debug.WriteLine($"Game {this.gameState.GameId} has corrupted state!");
                return;
            }

            var player = this.lobbyViewModel.PlayerData.FirstOrDefault(x => x.Id == msg.PlayerId);
            if (player == null)
            {
                Debug.WriteLine($"Game {this.gameState.GameId} has corrupted state!");
                return;
            }

            this.lobbyViewModel.PlayerData.Remove(player);
        }

        private async void OnPlayerHasConnected(NewPlayerConnected msg)
        {
            if (msg.GameId != this.gameState.GameId)
                return;

            if (this.lobbyViewModel == null)
            {
                Debug.WriteLine($"Game {this.gameState.GameId} has corrupted state!");
                return;
            }

            var playerData = await this.playerDataService.GetPlayerDataAsync(msg.NewPlayerId);
            this.lobbyViewModel.PlayerData.Add(playerData);
        }

        private async void OnGameHasStarted(GameHasStarted msg)
        {
            if (msg.State.GameId != this.gameState.GameId)
                return;

            gameState = msg.State;
            await LoadDataAsync();
        }
    }
}