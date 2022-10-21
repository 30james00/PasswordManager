using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfigurations;

public class SavedPasswordEntityTypeConfiguration : IEntityTypeConfiguration<SavedPassword>
{
    public void Configure(EntityTypeBuilder<SavedPassword> builder)
    {
        builder.Property(x => x.Password).HasMaxLength(256).IsRequired();
        builder.Property(x => x.WebAddress).HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Login).HasMaxLength(30);
        builder.Property(x => x.AccountId).IsRequired();
    }
}