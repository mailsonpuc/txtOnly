using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using txtOnly.Infrastructure.Identity;

namespace txtOnly.Infrastructure.EntitiesConfiguration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(user => user.NormalizedUserName)
            .IsUnique(false)
            .HasFilter("[NormalizedUserName] IS NOT NULL");
    }
}
