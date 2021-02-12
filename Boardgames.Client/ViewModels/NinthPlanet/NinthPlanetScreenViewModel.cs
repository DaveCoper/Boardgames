using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Boardgames.Client.Services;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;

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

        private NinthPlanetClient client;
        private object currentView;

        [Obsolete("Used by WPF designer", true)]
        public NinthPlanetScreenViewModel() : base(Resources.Strings.PlanetNine_Title)
        {
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
            this.gameInfo = gameInfo;
            this.gameState = gameState;
        }

        public int GameId => gameId;

        public NinthPlanetClient Client
        {
            get => client;
            private set => Set(ref client, value);
        }

        public object CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }

        public async void BeginRound()
        {
            await this.ninthPlanetService.BeginRoundAsync(this.gameId);
        }

        protected override async Task LoadDataInternalAsync()
        {
            if (this.gameInfo == null)
            {
                this.gameInfo = await this.gameInfoService.GetGameInfoAsync(GameId);
            }

            if (this.gameInfo.GameType != GameType.NinthPlanet)
            {
                throw new InvalidOperationException();
            }

            if (this.gameState == null)
            {
                this.gameState = await ninthPlanetService.JoinGameAsync(this.gameId);
            }

            if (this.Client == null)
            {
                this.Client = new NinthPlanetClient(this.gameInfo, playerDataService, null);
                this.Client.PropertyChanged += OnClientPropertyChanged;
                this.UpdateCurrentView();
            }

            await this.Client.UpdateStateAsync(gameState);
        }

        private void OnClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Client.GameLobby):
                case nameof(Client.CurrentRound):
                    this.UpdateCurrentView();
                    break;
            }
        }
        private void UpdateCurrentView()
        {
            CurrentView = Client.CurrentRound == null ? Client.GameLobby : Client.CurrentRound;
        }
    }
}