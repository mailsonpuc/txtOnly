using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using txtOnly.Domain.Entities;

namespace txtOnly.Infrastructure.EntitiesConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.Bio)
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasMany(x => x.Posts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasMany(x => x.Followers)
            .WithOne(x => x.Following)
            .HasForeignKey(x => x.FollowingId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasMany(x => x.Following)
            .WithOne(x => x.Follower)
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
