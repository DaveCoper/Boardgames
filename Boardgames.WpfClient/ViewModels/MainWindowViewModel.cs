using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Boardgames.Client.Services;
using Boardgames.Client.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Boardgames.WpfClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IAsyncLoad
    {
        private readonly IPlayerDataService playerDataService;

        private PlayerDataViewModel userInfo;

        private object content;

        [Obsolete("Designer use only", true)]
        public MainWindowViewModel()
        {
            this.MenuButtons = new ObservableCollection<MenuButtonViewModel>
            {
                new MenuButtonViewModel("Join game", new RelayCommand(() => { }, true)),
                new MenuButtonViewModel("Create game", new RelayCommand(() => { }, true)),
            };
        }

        public MainWindowViewModel(
            Func<CreateGameViewModel> createGameVmFactory,
            IPlayerDataService playerDataService)
        {
            if (createGameVmFactory is null)
            {
                throw new ArgumentNullException(nameof(createGameVmFactory));
            }

            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.MenuButtons = new ObservableCollection<MenuButtonViewModel>
            {
                new MenuButtonViewModel("Join game", new RelayCommand(() => { }, true)),
                new MenuButtonViewModel("Create game", new RelayCommand(() => this.Content = createGameVmFactory(), true)),
            };
        }

        public PlayerDataViewModel UserInfo
        {
            get => userInfo;
            set => Set(ref userInfo, value);
        }

        public ObservableCollection<MenuButtonViewModel> MenuButtons { get; set; }

        public object Content
        {
            get => content;
            set => Set(ref content, value);
        }

        public async Task LoadDataAsync()
        {
            var playerData = await playerDataService.GetMyDataAsync();
            this.UserInfo = new PlayerDataViewModel(playerData);
        }
    }
}