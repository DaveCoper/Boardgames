using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Client.Services;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class GameBrowserViewModel : ScreenViewModel, IAsyncLoad
    {
        private readonly IGameInfoService gameInfoService;

        private readonly IPlayerDataService playerDataService;

        private readonly Func<GameInfo, PlayerData, GameInfoViewModel> gameInfoVmFactory;

        private ObservableCollection<GameInfoViewModel> publicGames;

        [Obsolete("Used by WPF designer", true)]
        public GameBrowserViewModel() : base("Join game")
        {
            publicGames = new ObservableCollection<GameInfoViewModel>();
        }

        public GameBrowserViewModel(
            IGameInfoService gameInfoService,
            IPlayerDataService playerDataService,
            Func<GameInfo, PlayerData, GameInfoViewModel> gameInfoVmFactory,
            IMessenger messenger) : base("Join game", messenger)
        {
            this.gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
            this.playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            this.gameInfoVmFactory = gameInfoVmFactory ?? throw new ArgumentNullException(nameof(gameInfoVmFactory));

            this.publicGames = new ObservableCollection<GameInfoViewModel>();
        }

        public ObservableCollection<GameInfoViewModel> PublicGames
        {
            get => publicGames;
            set => Set(ref publicGames, value);
        }

        public async Task LoadDataAsync()
        {
            var games = await this.gameInfoService.GetPublicGamesAsync();
            var ownerIds = games.Select(x => x.OwnerId).Distinct().ToList();

            var ownerData = await playerDataService.GetPlayerDataAsync(ownerIds);
            var ownerDict = ownerData.ToDictionary(x => x.Id);

            foreach (var owner in ownerDict)
            {
                Console.WriteLine($"Owner key: {owner.Key}, value:{owner.Value}");
            }

            PublicGames = new ObservableCollection<GameInfoViewModel>(games.Select(x => gameInfoVmFactory(x, ownerDict[x.OwnerId])));
        }
    }
}