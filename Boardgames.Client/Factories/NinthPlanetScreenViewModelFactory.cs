using Boardgames.Client.Services;
using Boardgames.Client.ViewModels.NinthPlanet;
using Boardgames.Common.Services;
using Boardgames.NinthPlanet.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;

namespace Boardgames.Client.Factories
{
    public class NinthPlanetScreenViewModelFactory : INinthPlanetScreenViewModelFactory
    {
        private readonly IPlayerDataProvider playerDataProvider;

        private readonly INinthPlanetService ninthPlanetService;

        private readonly IMessenger messenger;

        private readonly ILoggerFactory loggerFactory;

        public NinthPlanetScreenViewModelFactory(
            IPlayerDataProvider playerDataProvider,
            INinthPlanetService ninthPlanetService,
            IMessenger messenger,
            ILoggerFactory loggerFactory)
        {
            this.playerDataProvider = playerDataProvider ?? throw new System.ArgumentNullException(nameof(playerDataProvider));
            this.ninthPlanetService = ninthPlanetService ?? throw new System.ArgumentNullException(nameof(ninthPlanetService));
            this.messenger = messenger ?? throw new System.ArgumentNullException(nameof(messenger));
            this.loggerFactory = loggerFactory ?? throw new System.ArgumentNullException(nameof(loggerFactory));
        }

        public NinthPlanetScreenViewModel CreateInstance(int ownerId, int gameId, GameState state)
        {
            return new NinthPlanetScreenViewModel(
                       ownerId,
                       gameId,
                       state,
                       playerDataProvider,
                       ninthPlanetService,
                       messenger,
                       loggerFactory.CreateLogger<NinthPlanetScreenViewModel>());
        }
    }
}