using Boardgames.Web.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boardgames.Web.Server.Data.EntityMappings
{
    public class NinthPlanetGameStateConfiguration : IEntityTypeConfiguration<NinthPlanetGameState>
    {
        public void Configure(EntityTypeBuilder<NinthPlanetGameState> gameStateBuilder)
        {
            gameStateBuilder
                .HasKey(x => x.GameId);

            gameStateBuilder
                .Property(x => x.GameId)
                .ValueGeneratedNever();

            gameStateBuilder
                .HasOne(x => x.GameInfo)
                .WithOne()
                .HasForeignKey<NinthPlanetGameState>(x => x.GameId)
                .Metadata.DeleteBehavior = DeleteBehavior.Restrict;

            gameStateBuilder
                .Ignore(x => x.BoardState);

            gameStateBuilder
                .Ignore(x => x.LobbyState);
        }
    }
}