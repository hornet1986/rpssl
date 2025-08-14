using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rpssl.Domain.GameResults;

namespace Rpssl.Infrastructure.GameResults;

public class GameResultConfiguration : IEntityTypeConfiguration<GameResult>
{
    public void Configure(EntityTypeBuilder<GameResult> builder)
    {
        builder.ToTable("GameResults");
        builder.HasKey(gr => gr.Id);

        builder.Property(gr => gr.PlayedAt)
            .IsRequired();

        builder.Property(gr => gr.Outcome)
            .HasConversion<string>()       // store enum as string for readability and portability
            .HasMaxLength(16)
            .IsRequired();

        builder.HasIndex(gr => gr.PlayedAt);
        builder.HasIndex(gr => gr.Outcome);

        builder.HasOne(gr => gr.PlayerChoice)
            .WithMany()
            .HasForeignKey(gr => gr.PlayerChoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gr => gr.ComputerChoice)
            .WithMany()
            .HasForeignKey(gr => gr.ComputerChoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
