using System;
using Boardgames.Client.Messages;
using Boardgames.Client.Services;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Boardgames.Client.ViewModels
{
    public class GameInfoViewModel
    {
        private readonly PlayerData ownerData;

        private readonly IIconUriProvider iconUriBuilder;

        private readonly IMessenger messenger;

        private GameInfo gameInfo;

        public GameInfoViewModel(
            GameInfo gameInfo, 
            PlayerData ownerData, 
            IIconUriProvider iconUriBuilder, 
            IMessenger messenger)
        {
            this.gameInfo = gameInfo ?? throw new ArgumentNullException(nameof(gameInfo));
            this.ownerData = ownerData ?? throw new ArgumentNullException(nameof(ownerData));
            this.iconUriBuilder = iconUriBuilder ?? throw new ArgumentNullException(nameof(iconUriBuilder));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

            this.JoinGameCommand = new RelayCommand(JoinGame);
        }

        public RelayCommand JoinGameCommand { get; }

        public int GameId => gameInfo.Id;

        public string CreatedBy => ownerData.Name;

        public string GameName => gameInfo.GameType.ToString();

        public Uri GameIcon => iconUriBuilder.GetIconUri(gameInfo.GameType, 32);

        public GameType GameType => gameInfo.GameType;

        public string Name => gameInfo.Name;

        public DateTime CreatedAt => gameInfo.CreatedAt;

        private void JoinGame()
        {
            this.messenger.Send(new OpenGame
            {
                GameId = this.gameInfo.Id,
                GameState = null,
                GameOwnerId = this.ownerData.Id,
                GameType = this.gameInfo.GameType,
            });
        }
    }
}