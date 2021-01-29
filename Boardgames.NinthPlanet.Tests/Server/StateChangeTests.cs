using System.Collections.Generic;
using Boardgames.Common.Messages;
using Boardgames.NinthPlanet.Models;
using NUnit.Framework;

namespace Boardgames.NinthPlanet.Tests.Server
{
    [TestFixture(Category = nameof(NinthPlanetServer))]
    public class StateChangeTests : ServerTests
    {
        [Test]
        public void BeginNewRound()
        {
            var rng = TestContext.CurrentContext.Random;
            var players = new List<int>();
            for (int i = 0; i < 5; ++i)
            {
                players.Add(rng.Next(1, 1000));
            }

            var gameOwner = players[0];

            var gameInfo = CreateGameInfo(gameOwnerId: gameOwner);

            var savedState = new SavedGameState
            {
                GameInfo = gameInfo,
                PlayersInLobby = players,
                RoundState = null,
            };

            var game = this.GameFactory.CreateGameFromState(savedState);
            var gameMessanger = NSubstitute.Substitute.For<IGameMessenger>();

            game.BeginRound(gameOwner, gameMessanger);

            savedState = game.SaveCurrentState();

            var roundState = savedState.RoundState;
            Assert.NotNull(roundState);

            Assert.Null(roundState.ColorOfCurrentTrick);
            Assert.Contains(roundState.CaptainPlayerId, players);
            Assert.True(players.TrueForAll(x => roundState.PlayerOrder.Contains(x)));
            Assert.True(players.TrueForAll(x => roundState.PlayerStates.ContainsKey(x)));
        }
    }
}