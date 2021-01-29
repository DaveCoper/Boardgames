using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Models;
using Boardgames.NinthPlanet.Tests.Utilitites;
using NUnit.Framework;

namespace Boardgames.NinthPlanet.Tests.Game
{
    [TestFixture]
    public class GameTests : Server.ServerTests
    {
        [Test]
        [Repeat(50)]
        public async Task PlayGameSessionAsync()
        {
            var rng = TestContext.CurrentContext.Random;
            int gameId = rng.Next();

            List<PlayerData> players = Enumerable.Range(1, 5)
                .Select(x => new PlayerData
                {
                    Id = x * 100,
                    Name = $"Test player {x}"
                })
                .ToList();

            // game owner must go first
            var gameOwner = players[0];

            var clients = players.ToDictionary(
                x => x.Id,
                x => new NinthPlanetClient(
                    gameId,
                    new MemoryPlayerDataProvider(x.Id, players),
                    null));

            var gameInfo = CreateGameInfo(gameId: gameId, gameOwnerId: gameOwner.Id);
            var messageRouter = new TestMessengeRouter();
            var gameServer = GameFactory.CreateNewGame(gameInfo);

            foreach (var player in players)
            {
                var gameState = gameServer.JoinGame(player.Id, messageRouter);
                var client = clients[player.Id];
                await client.UpdateStateAsync(gameState);
                messageRouter.AddClient(player.Id, client);

                await messageRouter.FlushAsync();
            }

            foreach (var player in players)
            {
                var client = clients[player.Id];

                Assert.Null(client.CurrentRound);
                Assert.NotNull(client.GameLobby);

                // this is possible because all PlayerData come from same list.
                Assert.True(players.All(x => client.GameLobby.ConnectedPlayers.Contains(x)));
            }

            gameServer.BeginRound(gameOwner.Id, messageRouter);
            await messageRouter.FlushAsync();

            foreach (var player in players)
            {
                var client = clients[player.Id];
                Assert.NotNull(client.CurrentRound);
            }

            Assert.AreEqual(
                40,
                clients.Values
                    .SelectMany(x => x.CurrentRound.UserState.CardsInHand)
                    .Distinct()
                    .Count());

            var serverState = gameServer.SaveCurrentState();
            while (serverState.RoundState != null)
            {
                var currentPlayerId = serverState.RoundState.CurrentPlayerId;

                // take goals
                if (serverState.RoundState.AvailableGoals.Count > 0)
                {
                    gameServer.TakeTaskCard(currentPlayerId, serverState.RoundState.AvailableGoals.First(), messageRouter);
                }
                else
                {
                    var client = clients[currentPlayerId];
                    var cardsInHand = client.CurrentRound.UserState.CardsInHand;

                    if (client.CurrentRound.ColorOfCurrentTrick == null)
                    {
                        for (int i = 0; i < cardsInHand.Count; ++i)
                        {
                            var card = cardsInHand[i];
                            if (card.Color != CardColor.Rocket || i == cardsInHand.Count - 1)
                            {
                                gameServer.PlayCard(currentPlayerId, card, messageRouter);
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < cardsInHand.Count; ++i)
                        {
                            var card = cardsInHand[i];
                            if (card.Color == client.CurrentRound.ColorOfCurrentTrick || i == cardsInHand.Count - 1)
                            {
                                gameServer.PlayCard(currentPlayerId, card, messageRouter);
                                break;
                            }
                        }
                    }
                }

                await messageRouter.FlushAsync();
                serverState = gameServer.SaveCurrentState();
            }
        }
    }
}