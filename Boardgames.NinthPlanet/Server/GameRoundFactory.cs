using System.Collections.Generic;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet.Server
{
    public class GameRoundFactory : IGameRoundFactory
    {
        private readonly ILoggerFactory loggerFactory;

        public GameRoundFactory() : this(null)
        {
        }

        public GameRoundFactory(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        }

        public GameRound CreateGameRound(
            GameInfo gameInfo,
            SavedRoundState roundState)
        {
            return new GameRound(
                gameInfo.Id,
                roundState,
                loggerFactory.CreateLogger<GameRound>());
        }
    }
}