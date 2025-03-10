using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CloudStorage.Domain.Authorization.Entities;

namespace CloudStorage.Database.Accounts.Configurations;

internal class AccountsConfiguration : IEntityTypeConfiguration<AccountInfo>
{
    public void Configure(EntityTypeBuilder<AccountInfo> builder)
    {
        builder.ToTable(nameof(AccountInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.HasIndex(item => item.Email).IsUnique();
        
        builder.Property(item => item.RefreshToken).IsRequired(false);
        builder.Property(item => item.Email).HasMaxLength(100).IsRequired();
        builder.Property(item => item.UserUuid).IsRequired();

        builder.Property(item => item.Role).HasConversion(
            value => value.ToString(),
            value => (AccountRole)Enum.Parse(typeof(AccountRole), value));
    }
}