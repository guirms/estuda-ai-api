using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class LayoutMapping : IEntityTypeConfiguration<Layout>
    {
        public void Configure(EntityTypeBuilder<Layout> builder)
        {
            builder.ToTable("Layouts")
                .HasIndex(l => l.Name);

            builder.Property(l => l.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(l => l.Ip)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(l => l.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(l => l.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasOne(l => l.Product)
                .WithMany(p => p.Layouts)
                .HasForeignKey(l => l.ProductId)
                .IsRequired();
        }
    }
}
