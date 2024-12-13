using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pinterest.Domain.Authorization.Entities;

namespace Pinterest.Database.Accounts.Configurations;

public class AccountsConfiguration : IEntityTypeConfiguration<AccountInfo>
{
    public void Configure(EntityTypeBuilder<AccountInfo> builder)
    {
        builder.ToTable(nameof(AccountInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.HasIndex(item => item.Email).IsUnique();
        
        builder.Property(item => item.Email).HasMaxLength(100).IsRequired();
    }
}