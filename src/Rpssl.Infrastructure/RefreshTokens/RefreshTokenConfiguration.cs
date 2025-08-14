using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rpssl.Domain.RefreshTokens;
using Rpssl.Domain.Users;

namespace Rpssl.Infrastructure.RefreshTokens;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(64); // SHA256 hex length
        builder.Property(rt => rt.SessionId).IsRequired().HasMaxLength(64);
        builder.Property(rt => rt.CreatedAt).IsRequired();
        builder.Property(rt => rt.ExpiresAt).IsRequired();
        builder.HasIndex(rt => rt.TokenHash).IsUnique();
        builder.HasIndex(rt => new { rt.UserId, rt.SessionId });
        builder.HasOne<User>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
