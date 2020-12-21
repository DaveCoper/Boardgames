using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boardgames.Common.Exceptions;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Boardgames.NinthPlanet.Tests
{
    public class NinthPlanetServerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddOwnerToPlayersOnStart()
        {
            Randomizer rng = TestContext.CurrentContext.Random;
            int gameId = rng.Next(1, 50000);
            int gameOwnerId = rng.Next(1, 50000);

            var gameInfo = CreateGameInfo(gameId, gameOwnerId);

            var gameState = new GameState
            {
                GameId = gameId,
                LobbyState = new LobbyState(),
            };

            var game = new NinthPlanetServer(gameInfo, gameState);
            var state = game.GetGameState(gameOwnerId);

            Assert.NotNull(state.LobbyState);
            Assert.NotNull(state.LobbyState.ConnectedPlayers);
            Assert.Contains(gameOwnerId, state.LobbyState.ConnectedPlayers);
        }

        [Test]
        public void PlayersJoinsLobby()
        {
            Randomizer rng = TestContext.CurrentContext.Random;
            int gameId = rng.Next(1, 50000);
            int gameOwnerId = rng.Next(1, 50000);
            int secondPlayerId = rng.Next(1, 50000);
            while (secondPlayerId == gameOwnerId)
            {
                secondPlayerId = rng.Next(1, 50000);
            }

            var gameInfo = CreateGameInfo(gameId, gameOwnerId);

            var gameState = new GameState
            {
                GameId = gameId,
                LobbyState = new LobbyState(),
            };

            var messenger = Substitute.For<IGameMessenger>();
            var game = new NinthPlanetServer(gameInfo, gameState);

            game.JoinGame(secondPlayerId, messenger);
            var state = game.GetGameState(gameOwnerId);

            Assert.NotNull(state.LobbyState);
            Assert.NotNull(state.LobbyState.ConnectedPlayers);
            Assert.Contains(secondPlayerId, state.LobbyState.ConnectedPlayers);
        }

        [Test]
        public void ThrowsWhenPlayersJoinsLobbyAndGameIsFull()
        {
            Randomizer rng = TestContext.CurrentContext.Random;
            int gameId = rng.Next(1, 50000);
            int gameOwnerId = rng.Next(1, 50000);
            GameInfo gameInfo = CreateGameInfo(gameId, gameOwnerId);

            var gameState = new GameState
            {
                GameId = gameId,
                LobbyState = new LobbyState(),
            };

            var messenger = Substitute.For<IGameMessenger>();

            var game = new NinthPlanetServer(gameInfo, gameState);
            var playersInGame = new HashSet<int>();
            playersInGame.Add(gameOwnerId);

            while (playersInGame.Count < gameInfo.MaximumNumberOfPlayers)
            {
                int anotherPlayerId = rng.Next(1, 50000);
                if (playersInGame.Add(anotherPlayerId))
                {
                    game.JoinGame(anotherPlayerId, messenger);
                }
            }

            Assert.Throws<GameIsFullException>(() => game.JoinGame(-5, messenger));
        }

        private static GameInfo CreateGameInfo(int gameId, int gameOwnerId)
        {
            return new GameInfo()
            {
                Id = gameId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                GameType = GameType.NinthPlanet,
                IsPublic = true,
                MaximumNumberOfPlayers = 5,
                Name = "Test game",
                OwnerId = gameOwnerId,
            };
        }
    }
}