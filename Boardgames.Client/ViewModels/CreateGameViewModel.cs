using Boardgames.Client.Brookers;
using Boardgames.Client.Messages;
using Boardgames.Client.Models;
using Boardgames.Client.Services;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Boardgames.Client.ViewModels
{
    public class CreateGameViewModel : ScreenViewModel, IAsyncLoad
    {
        private readonly IIconUriProvider iconUriProvider;

        private readonly IPlayerDataService playerDataService;

        private readonly IWebApiBrooker webApiBrooker;

        private PlayerData playerData;

        private ObservableCollection<GameViewModel> availableGames;

        private object gameSettings;

        private GameViewModel selectedGame;

        [Obsolete("Designer use only.", true)]
        public CreateGameViewModel() : base(Resources.Strings.CreateGame_Title)
        {
        }

        public CreateGameViewModel(
            IIconUriProvider iconUriProvider,
            IPlayerDataService playerDataService,
            IWebApiBrooker webApiBrooker,
            IMessenger messenger) : base(Resources.Strings.CreateGame_Title, messenger)
        {
            this.iconUriProvider = iconUriProvider ?? throw new ArgumentNullException(nameof(iconUriProvider));
            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.webApiBrooker = webApiBrooker ?? throw new ArgumentNullException(nameof(webApiBrooker));
            this.CreateGameCommand = new RelayCommand(CreateGame);
            this.AvailableGames = new ObservableCollection<GameViewModel>();
        }

        public ObservableCollection<GameViewModel> AvailableGames
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

        public GameViewModel SelectedGame
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

        public async Task LoadDataAsync()
        {
            this.playerData = await playerDataService.GetMyDataAsync();
            this.AvailableGames = new ObservableCollection<GameViewModel>
                        {
                            new GameViewModel {
                                Type = GameType.NinthPlanet,
                                Name = Boardgames.Client.Resources.Strings.PlanetNine_Title,
                                Icon32x32 =  iconUriProvider.GetIconUri(GameType.NinthPlanet, 32),
                                Icon128x128 =  iconUriProvider.GetIconUri(GameType.NinthPlanet, 128),
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
            this.MessengerInstance.Send(new SubscribeToGameMessages { GameId = gameState.GameId });
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