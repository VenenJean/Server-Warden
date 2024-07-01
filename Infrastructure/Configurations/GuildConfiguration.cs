using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class GuildConfiguration : EntityConfiguration<Guild>
{
    public override void Configure(EntityTypeBuilder<Guild> builder)
    {
        base.Configure(builder);
        builder.HasIndex(e => e.GuildId).IsUnique();
    }
}