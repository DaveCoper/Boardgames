using System;
using Boardgames.Game.Models;
using Boardgames.Web.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Boardgames.Web.Server.Data.EntityMappings
{
    public class DbGameInfoConfiguration : IEntityTypeConfiguration<DbGameInfo>
    {
        public void Configure(EntityTypeBuilder<DbGameInfo> builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.GameType)
                .IsRequired()
                .HasMaxLength(128);

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.CreatedGames)
                .HasForeignKey(x => x.OwnerId)
                .Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}