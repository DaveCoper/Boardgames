using System;
using Boardgames.Common.Models;
using Boardgames.NinthPlanet.Server;

namespace Boardgames.NinthPlanet.Tests.Server
{
    public class ServerTests
    {
        protected GameServerFactory GameFactory { get; } = new GameServerFactory(new GameRoundFactory());

        protected GameInfo CreateGameInfo(
            int gameId = 1,
            int gameOwnerId = 1)
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