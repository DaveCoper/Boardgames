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
    public class PlayCardTests
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
            HashSet<int> players = new HashSet<int>();
            players.Add(gameOwnerId);

            while (players.Count < 5)
            {
                players.Add(rng.Next(1, 50000));
            }

            var gameInfo = CreateGameInfo(gameId, gameOwnerId);

            var gameState = new GameState
            {
                GameId = gameId,
                LobbyState = new LobbyState(),
                BoardState = new BoardState
                {
                    CaptainId = gameOwnerId,
                    HelpIsAvailable = true,
                    AvailableGoals =  new List<TaskCard>(),
                }
            };

            var game = new NinthPlanetServer(gameInfo, gameState);
            var state = game.GetGameState(gameOwnerId);

            Assert.NotNull(state.LobbyState);
            Assert.NotNull(state.LobbyState.ConnectedPlayers);
            Assert.Contains(gameOwnerId, state.LobbyState.ConnectedPlayers);
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