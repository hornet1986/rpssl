using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rpssl.Domain.Choices;

namespace Rpssl.Infrastructure.Choices;

internal sealed class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
{
    public void Configure(EntityTypeBuilder<Choice> builder)
    {
        builder.ToTable("Choices");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(32);
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
