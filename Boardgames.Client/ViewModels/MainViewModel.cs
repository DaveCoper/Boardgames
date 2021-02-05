using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Factories;
using Boardgames.Client.Messages;
using Boardgames.Client.ViewModels.NinthPlanet;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class MainViewModel : ViewModelBase, IAsyncLoad
    {
        private readonly CreateGameViewModel createGameViewModel;
        private readonly GameBrowserViewModel gameBrowserViewModel;
        private readonly INinthPlanetScreenViewModelFactory ninthPlanetScreenViewModelFactory;
        private readonly IPlayerDataProvider playerDataProvider;
        private readonly IMessenger messenger;
        private PlayerData currentUser;

        private ContentViewModel activeScreen;

        public MainViewModel(
            HomeViewModel homeViewModel,
            CreateGameViewModel createGameViewModel,
            GameBrowserViewModel gameBrowserViewModel,
            INinthPlanetScreenViewModelFactory ninthPlanetScreenViewModelFactory,
            IPlayerDataProvider playerDataProvider,
            IMessenger messenger)
        {
            this.playerDataProvider = playerDataProvider ?? throw new ArgumentNullException(nameof(playerDataProvider));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            this.ActiveScreen = homeViewModel ?? throw new ArgumentNullException(nameof(homeViewModel));
            this.createGameViewModel = createGameViewModel ?? throw new ArgumentNullException(nameof(createGameViewModel));
            this.gameBrowserViewModel = gameBrowserViewModel ?? throw new ArgumentNullException(nameof(gameBrowserViewModel));
            this.ninthPlanetScreenViewModelFactory = ninthPlanetScreenViewModelFactory ?? throw new ArgumentNullException(nameof(ninthPlanetScreenViewModelFactory));

            this.Screens = new ObservableCollection<ContentViewModel>
            {
                homeViewModel,
                createGameViewModel,
                gameBrowserViewModel,
            };



            messenger.Register<OpenGame>(this, OnUserWantsToOpenGame);
        }

        public ObservableCollection<ContentViewModel> Screens { get; }

        public ContentViewModel ActiveScreen
        {
            get => activeScreen;
            set => Set(ref activeScreen, value);
        }

        public PlayerData CurrentUser
        {
            get => currentUser;
            set => Set(ref currentUser, value);
        }

        public async Task LoadDataAsync()
        {
            if (this.CurrentUser == null)
            {
                this.CurrentUser = await playerDataProvider.GetPlayerDataForCurrentUserAsync();
            }
        }

        private void OnUserWantsToOpenGame(OpenGame msg)
        {
            var existingScreen = (ContentViewModel)this.Screens.OfType<IGameViewModel>().FirstOrDefault(x => x.GameId == msg.GameId);
            if(existingScreen == null)
            {
                switch (msg.GameType)
                {
                    case GameType.NinthPlanet:
                        existingScreen = CreateNinthPlanetScreen(msg);
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                this.Screens.Add(existingScreen);
            }
        }

        private NinthPlanetScreenViewModel CreateNinthPlanetScreen(OpenGame msg)
        {
            return ninthPlanetScreenViewModelFactory.CreateInstance(msg.GameId);
        }
    }
}