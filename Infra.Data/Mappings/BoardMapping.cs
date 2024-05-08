using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class BoardMapping : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.ToTable("Boards")
                .HasIndex(b => b.Name);

            builder.HasKey(b => b.BoardId);

            builder.Property(b => b.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(b => b.ExamDateTime)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(b => b.ExamDateTime)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(b => b.DailyStudyTime)
                .HasColumnType("time(0)")
                .IsRequired();

            builder.Property(b => b.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Boards)
                .HasForeignKey(u => u.UserId)
                .IsRequired();

            builder.HasMany(b => b.Cards)
                .WithOne(c => c.Board)
                .IsRequired(false);
        }
    }
}
