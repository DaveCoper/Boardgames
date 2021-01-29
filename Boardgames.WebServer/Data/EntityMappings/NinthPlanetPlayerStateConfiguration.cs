using Boardgames.WebServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boardgames.WebServer.Data.EntityMappings
{
    public class NinthPlanetPlayerStateConfiguration : IEntityTypeConfiguration<NinthPlanetPlayerState>
    {
        public void Configure(EntityTypeBuilder<NinthPlanetPlayerState> playerStateBuilder)
        {
            playerStateBuilder
                .HasKey(x => new { x.PlayerId, x.GameId });

            playerStateBuilder
                .HasOne(x => x.Player)
                .WithOne()
                .HasForeignKey<NinthPlanetPlayerState>(x => x.PlayerId)
                .Metadata.DeleteBehavior = DeleteBehavior.Restrict;

            playerStateBuilder
                .Ignore(x => x.CardsInHand);

            playerStateBuilder
                .Property(x => x.SerializedCardsInHand)
                .UsePropertyAccessMode(PropertyAccessMode.Property);


            playerStateBuilder
                .Ignore(x => x.FinishedTasks);

            playerStateBuilder
                .Property(x => x.SerializedFinishedTasks)
                .UsePropertyAccessMode(PropertyAccessMode.Property);


            playerStateBuilder
                .Ignore(x => x.UnfinishedTasks);

            playerStateBuilder
                .Property(x => x.SerializedUnfinishedTasks)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}