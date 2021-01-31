using Boardgames.Client.Services;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Boardgames.Client.ViewModels.NinthPlanet
{
    public class NinthPlanetScreenViewModel : ContentViewModel, IAsyncLoad, IGameViewModel
    {
        private readonly int gameId;

        private readonly IPlayerDataProvider playerDataService;

        private readonly IGameInfoService gameInfoService;

        private readonly INinthPlanetService ninthPlanetService;

        private GameState gameState;

        private GameInfo gameInfo;

        private LobbyViewModel lobbyViewModel;

        private object currentView;

        [Obsolete("Used by WPF designer", true)]
        public NinthPlanetScreenViewModel() : base(Resources.Strings.PlanetNine_Title)
        {
            lobbyViewModel = new LobbyViewModel();
            this.CurrentView = lobbyViewModel;
        }

        public NinthPlanetScreenViewModel(
            int gameId,
            IPlayerDataProvider playerDataService,
            IGameInfoService gameInfoService,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger,
            ILogger<NinthPlanetScreenViewModel> logger)
            : base(Resources.Strings.PlanetNine_Title, messenger, logger)
        {
            this.gameId = gameId;
            if (gameState != null)
            {
                Debug.Assert(gameState.GameId == gameId, "Game ids are not the same!");
            }

            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
            this.ninthPlanetService = ninthPlanetService ?? throw new ArgumentNullException(nameof(ninthPlanetService));

            messenger.Register<GameHasStarted>(this, OnGameHasStarted);
            messenger.Register<NewPlayerConnected>(this, OnPlayerHasConnected);
            messenger.Register<PlayerHasLeft>(this, OnPlayerHasLeft);
        }

        public NinthPlanetScreenViewModel(
            int gameId,
            GameInfo gameInfo,
            GameState gameState,
            IPlayerDataProvider playerDataService,
            IGameInfoService gameInfoService,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger,
            ILogger<NinthPlanetScreenViewModel> logger)
            : this(gameId, playerDataService, gameInfoService, ninthPlanetService, messenger, logger)
        {
            this.gameId = gameId;
            this.gameInfo = gameInfo;
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
            if (gameInfo == null)
            {
                this.gameInfo = await this.gameInfoService.GetGameInfoAsync(GameId);
            }

            if (gameInfo.GameType != GameType.NinthPlanet)
            {
                throw new InvalidOperationException();
            }

            if (gameState == null)
            {
                this.gameState = await ninthPlanetService.JoinGameAsync(this.gameId);
            }

            var client = new NinthPlanetClient(this.GameId, playerDataService, null);
            await client.UpdateStateAsync(gameState);

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