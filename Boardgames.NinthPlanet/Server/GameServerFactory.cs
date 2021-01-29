using System;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boardgames.NinthPlanet.Server
{
    public class GameServerFactory : IGameServerFactory
    {
        private readonly IGameRoundFactory gameRoundFactory;

        private readonly ILoggerFactory loggerFactory;

        public GameServerFactory(IGameRoundFactory gameRoundFactory) : this(gameRoundFactory, null)
        {
        }

        public GameServerFactory(IGameRoundFactory gameRoundFactory, ILoggerFactory loggerFactory)
        {
            this.gameRoundFactory = gameRoundFactory ?? throw new ArgumentNullException(nameof(gameRoundFactory));
            this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        }

        public NinthPlanetServer CreateNewGame(GameInfo gameInfo)
        {
            if (gameInfo is null)
            {
                throw new ArgumentNullException(nameof(gameInfo));
            }

            if (gameInfo.GameType != GameType.NinthPlanet)
            {
                throw new ArgumentException($"Expected game type to be {GameType.NinthPlanet} but {gameInfo.GameType} was found.", nameof(gameInfo));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            var gameLobby = new GameLobby(
                gameInfo.Id,
                gameInfo.MaximumNumberOfPlayers,
                loggerFactory.CreateLogger<GameLobby>());

            gameLobby.AddPlayer(gameInfo.OwnerId);
            return new NinthPlanetServer(
                gameInfo,
                gameLobby,
                gameRoundFactory,
                loggerFactory.CreateLogger<NinthPlanetServer>());
        }

        public NinthPlanetServer CreateGameFromState(SavedGameState gameState)
        {
            var gameInfo = gameState.GameInfo;
            if (gameInfo is null)
            {
                throw new ArgumentException("Game info is empty.", nameof(gameState));
            }

            if (gameInfo.GameType != GameType.NinthPlanet)
            {
                throw new ArgumentException($"Expected game type to be {GameType.NinthPlanet} but {gameInfo.GameType} was found.", nameof(gameState));
            }

            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            var gameLobby = new GameLobby(
                gameInfo.Id,
                gameInfo.MaximumNumberOfPlayers,
                gameState.PlayersInLobby,
                loggerFactory.CreateLogger<GameLobby>());

            gameLobby.AddPlayer(gameInfo.OwnerId);

            GameRound gameRound = null;
            if (gameState.RoundState != null)
            {
                gameRound = gameRoundFactory.CreateGameRound(gameInfo, null);
            }

            return new NinthPlanetServer(
                gameInfo,
                gameLobby,
                gameRound,
                gameRoundFactory,
                loggerFactory.CreateLogger<NinthPlanetServer>());
        }
    }
}