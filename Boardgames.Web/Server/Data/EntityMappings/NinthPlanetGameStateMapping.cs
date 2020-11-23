using System;
using Boardgames.NinthPlanet.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boardgames.Web.Server.Data.EntityMappings
{
    public static class NinthPlanetGameStateMapping
    {
        public static void MapEntity(EntityTypeBuilder<GameState> gameStateBuilder)
        {
            if (gameStateBuilder is null)
            {
                throw new ArgumentNullException(nameof(gameStateBuilder));
            }

            const string keyName = "GameInfoId";
            gameStateBuilder
                .Property<int>(keyName)
                .ValueGeneratedNever();

            gameStateBuilder.HasKey(keyName);

            gameStateBuilder
                .HasOne(x => x.GameInfo)
                .WithOne()
                .HasForeignKey(keyName);

            gameStateBuilder
                .Ignore(x => x.BoardState);
        }
    }
}