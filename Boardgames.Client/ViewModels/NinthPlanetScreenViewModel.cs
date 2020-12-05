using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Services;
using Boardgames.NinthPlanet.Messages;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class NinthPlanetScreenViewModel : ScreenViewModel, IAsyncLoad
    {
        private const string ScreenLabel = "Ninth planet";

        private readonly int gameOwnerId;

        private readonly int gameId;

        private readonly IPlayerDataService playerDataService;

        private readonly INinthPlanetService ninthPlanetService;

        private GameState gameState;

        private NinthPlanetLobbyViewModel lobbyViewModel;

        private bool isNotBussy = true;

        private object currentView;

        [Obsolete("Used by WPF designer", true)]
        public NinthPlanetScreenViewModel() : base(ScreenLabel)
        {
            lobbyViewModel = new NinthPlanetLobbyViewModel();
            this.CurrentView = lobbyViewModel;
        }

        public NinthPlanetScreenViewModel(
            int gameOwnerId,
            int gameId,
            GameState gameState,
            IPlayerDataService playerDataService,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger)
            : base(ScreenLabel, messenger)
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

        public bool IsNotBussy
        {
            get => isNotBussy;
            set => Set(ref isNotBussy, value);
        }

        public object CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }

        public async Task LoadDataAsync()
        {
            IsNotBussy = false;
            try
            {
                if (gameState == null)
                {
                    gameState = await ninthPlanetService.JoinGameAsync(this.gameId);
                }

                if (gameState.BoardState != null)
                {
                    CurrentView = null;
                    return;
                }

                if (gameState.LobbyState != null)
                {
                    var myData = await this.playerDataService.GetMyDataAsync();
                    var playerData = await this.playerDataService.GetPlayerDataAsync(gameState.LobbyState.ConnectedPlayers);
                    lobbyViewModel = new NinthPlanetLobbyViewModel(playerData, myData.Id == gameOwnerId);
                    CurrentView = lobbyViewModel;
                    return;
                }
            }
            finally
            {
                IsNotBussy = true;
            }
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

        private void OnPlayerHasConnected(NewPlayerConnected msg)
        {
            if (msg.GameId != this.gameState.GameId)
                return;

            if (this.lobbyViewModel == null)
            {
                Debug.WriteLine($"Game {this.gameState.GameId} has corrupted state!");
                return;
            }

            this.playerDataService.GetPlayerDataAsync(msg.NewPlayerId)
                .ContinueWith(
                t => this.lobbyViewModel.PlayerData.Add(t.Result),
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnGameHasStarted(GameHasStarted msg)
        {
            if (msg.State.GameId != this.gameState.GameId)
                return;

            gameState = msg.State;
        }
    }
}