using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class SharedPasswordEntityTypeConfiguration : IEntityTypeConfiguration<SharedPassword>
{
    public void Configure(EntityTypeBuilder<SharedPassword> builder)
    {
        builder.Property(x => x.AccountId).IsRequired();
        builder.Property(x => x.SavedPasswordId).IsRequired();
    }
}