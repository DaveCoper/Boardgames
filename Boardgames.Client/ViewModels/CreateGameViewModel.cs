using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Client.Caches;
using Boardgames.Client.Messages;
using Boardgames.Client.Models;
using Boardgames.Client.Services;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class CreateGameViewModel : ScreenViewModel
    {
        private readonly IWebApiBrooker webApiBrooker;

        private readonly PlayerData playerData;

        private ObservableCollection<GameViewModel> availableGames;

        private object gameSettings;

        private GameViewModel selectedGame;

        [Obsolete("Designer use only.", true)]
        public CreateGameViewModel() : base("Create game")
        {
        }

        public CreateGameViewModel(
            IIconUriProvider iconUriProvider,
            IPlayerDataCache playerDataCache,
            IWebApiBrooker webApiBrooker,
            IMessenger messenger) : base("Create game", messenger)
        {
            if (iconUriProvider is null)
            {
                throw new ArgumentNullException(nameof(iconUriProvider));
            }

            if (playerDataCache is null)
            {
                throw new ArgumentNullException(nameof(playerDataCache));
            }

            this.webApiBrooker = webApiBrooker ?? throw new ArgumentNullException(nameof(webApiBrooker));

            this.CreateGameCommand = new RelayCommand(CreateGame);

            this.playerData = playerDataCache.CurrentUserData;

            this.AvailableGames = new ObservableCollection<GameViewModel>
                        {
                            new GameViewModel {
                                Type = GameType.NinthPlanet,
                                Name = "The Crew - Quest for Planet Nine",
                                Icon32x32 =  iconUriProvider.GetIconUri(GameType.NinthPlanet, 32),
                                Icon128x128 =  iconUriProvider.GetIconUri(GameType.NinthPlanet, 128),
                            }
                        };
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

        private void CreateGame()
        {
            if (this.SelectedGame == null)
            {
                return;
            }

            switch (this.SelectedGame.Type)
            {
                case GameType.NinthPlanet:
                    CreateNinthPlanetGame();
                    break;
            }
        }

        private void CreateNinthPlanetGame()
        {
            var controllerName = GameType.NinthPlanet.ToString();
            var task = webApiBrooker.PostAsync<NinthPlanet.Models.GameState, NinthPlanetNewGameOptions>(
                controllerName,
                (NinthPlanetNewGameOptions)GameSettings,
                "Create");

            task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    var result = t.Result;
                    var message = new OpenGame
                    {
                        GameId = result.GameId,
                        GameType = GameType.NinthPlanet,
                        GameState = result,
                    };

                    this.MessengerInstance.Send(new SubscribeToGameMessages { GameId = result.GameId });
                    this.MessengerInstance.Send(message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
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
                        MaxNumberOfPlayers = 3,
                        Name = $"{playerData.Name}'s Quest for Planet Nine"
                    };
                    break;
            }
        }
    }
}