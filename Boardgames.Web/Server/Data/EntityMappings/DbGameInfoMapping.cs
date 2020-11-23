using System;
using Boardgames.Web.Server.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boardgames.Web.Server.Data.EntityMappings
{
    public static class DbGameInfoMapping
    {
        public static void MapEntity(EntityTypeBuilder<DbGameInfo> gameStateBuilder)
        {
            if (gameStateBuilder is null)
            {
                throw new ArgumentNullException(nameof(gameStateBuilder));
            }

            gameStateBuilder.HasKey(x => x.Id);
            gameStateBuilder.HasOne(x=> x.Owner)
                .WithMany(x => x.CreatedGames)
                .HasForeignKey(x => x.OwnerId);
        }
    }
}