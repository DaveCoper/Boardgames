using System;
using System.Collections.ObjectModel;
using Boardgames.Client.Caches;
using Boardgames.Client.Messages;
using Boardgames.Client.ViewModels;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.WpfClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Func<int, GameState, NinthPlanetScreenViewModel> ninthPlanetScreenFactory;
        private PlayerDataViewModel userInfo;

        private ScreenViewModel selectedScreen;

        [Obsolete("Designer use only", true)]
        public MainWindowViewModel() : base()
        {
            this.AvailableScreens = new ObservableCollection<ScreenViewModel>
            {
                new ScreenViewModel("Join game"),
                new ScreenViewModel("Create game"),
            };

            this.UserInfo = new PlayerDataViewModel(new PlayerData { 
                Id = 10, 
                Name = "Player", 
                AvatarUri = new Uri("pack://application:,,,/Boardgames.WpfClient;component/Resources/Images/NinthPlanet128x128.png") });
        }

        public MainWindowViewModel(
            CreateGameViewModel createGameViewModel,
            Func<int, GameState, NinthPlanetScreenViewModel> ninthPlanetScreenFactory,
            IPlayerDataCache playerDataCache,
            IMessenger messenger) : base(messenger)
        {
            if (createGameViewModel is null)
            {
                throw new ArgumentNullException(nameof(createGameViewModel));
            }

            this.AvailableScreens = new ObservableCollection<ScreenViewModel>
            {
                new ScreenViewModel("Join game"),
                createGameViewModel,
            };

            this.UserInfo = new PlayerDataViewModel(playerDataCache.CurrentUserData);            
            this.ninthPlanetScreenFactory = ninthPlanetScreenFactory ?? throw new ArgumentNullException(nameof(ninthPlanetScreenFactory));
            
            messenger.Register<OpenGame>(this, OnOpenGameReceived);
        }

        public PlayerDataViewModel UserInfo
        {
            get => userInfo;
            set => Set(ref userInfo, value);
        }

        public ObservableCollection<ScreenViewModel> AvailableScreens { get; set; }

        public ScreenViewModel SelectedScreen
        {
            get => selectedScreen;
            set => Set(ref selectedScreen, value);
        }

        private void OnOpenGameReceived(OpenGame message)
        {
            ScreenViewModel newScreen;
            switch (message.GameType)
            {
                case GameType.NinthPlanet:
                    newScreen = ninthPlanetScreenFactory(
                        message.GameOwnerId,
                        (GameState)message.GameState);
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.AvailableScreens.Add(newScreen);
            this.SelectedScreen = newScreen;
        }
    }
}