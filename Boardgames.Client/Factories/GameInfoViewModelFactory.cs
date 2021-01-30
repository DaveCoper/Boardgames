using System;
using Boardgames.Client.Services;
using Boardgames.Client.ViewModels;
using Boardgames.Common.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;

namespace Boardgames.Client.Factories
{
    public class GameInfoViewModelFactory
    {
        private readonly IIconUriProvider iconUriBuilder;

        private readonly IMessenger messenger;

        private readonly ILoggerFactory loggerFactory;

        public GameInfoViewModelFactory(
            IIconUriProvider iconUriBuilder,
            IMessenger messenger,
            ILoggerFactory loggerFactory)
        {
            this.iconUriBuilder = iconUriBuilder ?? throw new ArgumentNullException(nameof(iconUriBuilder));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public GameInfoViewModel CreateInstance(GameInfo gameInfo, PlayerData gameOwner)
        {
            return new GameInfoViewModel(
                gameInfo,
                gameOwner,
                iconUriBuilder,
                messenger);
        }
    }
}