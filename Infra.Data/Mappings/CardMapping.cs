using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class CardMapping : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("Cards");

            builder.HasKey(c => c.BoardId);

            builder.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(c => c.TaskStatus)
                .IsRequired();

            builder.Property(c => c.Order)
                .IsRequired();

            builder.Property(c => c.StudyTime)
                .HasColumnType("time(0)")
                .IsRequired();

            builder.Property(c => c.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasOne(c => c.Board)
                .WithMany(b => b.Cards)
                .HasForeignKey(u => u.BoardId)
                .IsRequired();
        }
    }
}
