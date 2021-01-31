using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Factories;
using Boardgames.Client.Services;
using Boardgames.Common.Models;
using Boardgames.Common.Services;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.Client.ViewModels
{
    public class GameBrowserViewModel : ContentViewModel, IAsyncLoad
    {
        private readonly IGameInfoService gameInfoService;

        private readonly IPlayerDataProvider playerDataService;

        private readonly IGameInfoViewModelFactory gameInfoVmFactory;

        private readonly ILogger<GameBrowserViewModel> logger;

        private ObservableCollection<GameInfoViewModel> publicGames;

        [Obsolete("Used by WPF designer", true)]
        public GameBrowserViewModel() : base(Resources.Strings.GameBrowser_Title, null)
        {
            publicGames = new ObservableCollection<GameInfoViewModel>();
        }

        public GameBrowserViewModel(
            IGameInfoService gameInfoService,
            IPlayerDataProvider playerDataService,
            IGameInfoViewModelFactory gameInfoVmFactory,
            IMessenger messenger,
            ILogger<GameBrowserViewModel> logger) : base(Resources.Strings.GameBrowser_Title, messenger, logger)
        {
            this.gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.gameInfoVmFactory = gameInfoVmFactory ?? throw new ArgumentNullException(nameof(gameInfoVmFactory));
            this.logger = logger ?? NullLogger<GameBrowserViewModel>.Instance;

            this.publicGames = new ObservableCollection<GameInfoViewModel>();
        }

        public ObservableCollection<GameInfoViewModel> PublicGames
        {
            get => publicGames;
            set => Set(ref publicGames, value);
        }

        protected override async Task LoadDataInternalAsync()
        {
            var games = await this.gameInfoService.GetPublicGamesAsync();
            var ownerIds = games.Select(x => x.OwnerId).Distinct().ToList();

            var ownerData = await playerDataService.GetPlayerDataAsync(ownerIds);
            var ownerDict = ownerData.ToDictionary(x => x.Id);

            var publicGames = games
                .Select(x => gameInfoVmFactory.CreateInstance(x, ownerDict[x.OwnerId]))
                .ToList();

            PublicGames = new ObservableCollection<GameInfoViewModel>(publicGames);
        }
    }
}