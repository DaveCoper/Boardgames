using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Client.Messages;
using Boardgames.Client.Models;
using Boardgames.Client.Services;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;

namespace Boardgames.Client.ViewModels
{
    public class CreateGameViewModel : ContentViewModel, IAsyncLoad
    {
        private readonly IIconUriProvider iconUriBuilder;

        private readonly IPlayerDataProvider playerDataService;

        private readonly IWebApiBrooker webApiBrooker;

        private PlayerData playerData;

        private ObservableCollection<NewGameOptionsViewModel> availableGames;

        private object gameSettings;

        private NewGameOptionsViewModel selectedGame;

        [Obsolete("Designer use only.", true)]
        public CreateGameViewModel() : base(Resources.Strings.CreateGame_Title)
        {
        }

        public CreateGameViewModel(
            IIconUriProvider iconUriBuilder,
            IPlayerDataProvider playerDataService,
            IWebApiBrooker webApiBrooker,
            IMessenger messenger,
            ILogger<CreateGameViewModel> logger)
            : base(
                Resources.Strings.CreateGame_Title,
                messenger,
                logger)
        {
            this.iconUriBuilder = iconUriBuilder ?? throw new ArgumentNullException(nameof(iconUriBuilder));
            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.webApiBrooker = webApiBrooker ?? throw new ArgumentNullException(nameof(webApiBrooker));
            this.CreateGameCommand = new RelayCommand(CreateGame);
            this.AvailableGames = new ObservableCollection<NewGameOptionsViewModel>();
        }

        public ObservableCollection<NewGameOptionsViewModel> AvailableGames
        {
            get => availableGames;
            set
            {
                if (Set(ref availableGames, value))
                {
                    OnAvailableGamesChanged();
                }
            }
        }

        public RelayCommand CreateGameCommand { get; }

        public NewGameOptionsViewModel SelectedGame
        {
            get => selectedGame;
            set
            {
                if (Set(ref selectedGame, value))
                {
                    OnSelectedGameChanged();
                }
            }
        }

        public object GameSettings
        {
            get => gameSettings;
            set => Set(ref gameSettings, value);
        }

        protected override async Task LoadDataInternalAsync()
        {
            this.playerData = await playerDataService.GetPlayerDataForCurrentUserAsync();
            this.AvailableGames = new ObservableCollection<NewGameOptionsViewModel>
                        {
                            new NewGameOptionsViewModel {
                                Type = GameType.NinthPlanet,
                                Name = Resources.Strings.PlanetNine_Title,
                                Icon32x32 =  iconUriBuilder.GetIconUri(GameType.NinthPlanet, 32),
                                Icon128x128 =  iconUriBuilder.GetIconUri(GameType.NinthPlanet, 128),
                            }
                        };
        }

        private async void CreateGame()
        {
            if (this.SelectedGame == null)
            {
                return;
            }

            switch (this.SelectedGame.Type)
            {
                case GameType.NinthPlanet:
                    await CreateNinthPlanetGame();
                    break;
            }
        }

        private async Task CreateNinthPlanetGame()
        {
            var controllerName = GameType.NinthPlanet.ToString();
            var gameState = await webApiBrooker.PostAsync<Boardgames.NinthPlanet.Models.GameState, NinthPlanetNewGameOptions>(
                controllerName,
                (NinthPlanetNewGameOptions)GameSettings,
                "Create");

            var message = new OpenGame
            {
                GameId = gameState.GameId,
                GameType = GameType.NinthPlanet,
                GameState = gameState,
            };

            this.MessengerInstance.Send(message);
        }

        private void OnAvailableGamesChanged()
        {
            var allGames = this.AvailableGames;

            if (allGames != null && allGames.Count > 0)
            {
                if (!allGames.Contains(this.SelectedGame))
                {
                    this.SelectedGame = allGames.First();
                }
            }
            else
            {
                this.SelectedGame = null;
            }
        }

        private void OnSelectedGameChanged()
        {
            if (this.SelectedGame == null)
            {
                GameSettings = null;
                return;
            }

            switch (this.SelectedGame.Type)
            {
                case GameType.NinthPlanet:
                    GameSettings = new NinthPlanetNewGameOptions
                    {
                        IsPublic = true,
                        MaxNumberOfPlayers = 5,
                        Name = string.Format(Resources.Strings.PlanetNine_NewGameName, playerData.Name),
                    };
                    break;
            }
        }
    }
}