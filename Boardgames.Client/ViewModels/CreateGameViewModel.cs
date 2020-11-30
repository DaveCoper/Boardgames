using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Brookers;
using Boardgames.Client.Models;
using Boardgames.Client.Services;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class CreateGameViewModel : ViewModelBase
    {
        private readonly IWebApiBrooker webApiBrooker;

        private ObservableCollection<GameViewModel> availableGames;

        private object gameSettings;

        private GameViewModel selectedGame;

        private PlayerData playerData;

        [Obsolete("Designer use only.", true)]
        public CreateGameViewModel()
        {
        }

        public CreateGameViewModel(
            IIconUriProvider iconUriProvider,
            IPlayerDataService playerDataService,
            IWebApiBrooker webApiBrooker,
            IMessenger messenger) : base(messenger)
        {
            if (iconUriProvider is null)
            {
                throw new ArgumentNullException(nameof(iconUriProvider));
            }

            if (playerDataService is null)
            {
                throw new ArgumentNullException(nameof(playerDataService));
            }

            this.webApiBrooker = webApiBrooker ?? throw new ArgumentNullException(nameof(webApiBrooker));

            this.CreateGameCommand = new RelayCommand(CreateGame);

            playerDataService.GetMyDataAsync()
                .ContinueWith(
                    t =>
                    {
                        this.playerData = t.Result;

                        this.AvailableGames = new ObservableCollection<GameViewModel>
                        {
                            new GameViewModel {
                                Type = GameType.NinthPlanet,
                                Name = "The Crew - Quest for Planet Nine",
                                Icon32x32 =  iconUriProvider.GetIconUri(GameType.NinthPlanet, 32),
                                Icon128x128 =  iconUriProvider.GetIconUri(GameType.NinthPlanet, 128),
                            }
                        };
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
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
                    webApiBrooker.PostAsync<object, NinthPlanetNewGameOptions>("NinthPlanet", (NinthPlanetNewGameOptions)GameSettings, "Create");
                    break;
            }
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