using System.Collections.Generic;
using Boardgames.Common.Exceptions;
using Boardgames.Common.Messages;
using Boardgames.Common.Models;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Boardgames.NinthPlanet.Tests.Server
{
    [TestFixture(Category = nameof(NinthPlanetServer))]
    public class JoinServerTests : ServerTests
    {
        [Test]
        public void AddOwnerToPlayersOnStart()
        {
            Randomizer rng = TestContext.CurrentContext.Random;
            int gameId = rng.Next(1, 50000);
            int gameOwnerId = rng.Next(1, 50000);

            var gameInfo = CreateGameInfo(gameId, gameOwnerId);

            var game = GameFactory.CreateNewGame(gameInfo);
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
            var messenger = Substitute.For<IGameMessenger>();
            var game = GameFactory.CreateNewGame(gameInfo);

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
            var game = GameFactory.CreateNewGame(gameInfo);
            
            var messenger = Substitute.For<IGameMessenger>();

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
    }
}