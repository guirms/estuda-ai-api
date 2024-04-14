using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class EggCategoryMapping : IEntityTypeConfiguration<EggCategory>
    {
        public void Configure(EntityTypeBuilder<EggCategory> builder)
        {
            builder.ToTable("EggCategories")
                .HasIndex(e => new { e.Name });

            builder.HasKey(e => e.EggCategoryId);

            builder.Property(e => e.Category)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(15)
                .IsRequired(false);

            builder.Property(e => e.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.Property(e => e.UserId)
                .IsRequired();

            builder.HasOne(e => e.User)
               .WithMany(u => u.EggCategories)
               .HasForeignKey(e => e.UserId)
               .IsRequired();
        }
    }
}
