using CodeBreaker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBreaker.Data.Configurations;

internal class GameDbConfiguration : IEntityTypeConfiguration<Game> {
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(x => x.GameId);
        builder.HasPartitionKey(x => x.GameId);
    }
}
