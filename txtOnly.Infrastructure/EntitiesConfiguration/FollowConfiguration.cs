using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using txtOnly.Domain.Entities;

namespace txtOnly.Infrastructure.EntitiesConfiguration;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.HasKey(x => x.FollowId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Follower)
            .WithMany(x => x.Following)
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Following)
            .WithMany(x => x.Followers)
            .HasForeignKey(x => x.FollowingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.FollowerId, x.FollowingId })
            .IsUnique();
    }
}
