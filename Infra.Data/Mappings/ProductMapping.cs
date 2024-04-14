using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products")
                .HasIndex(p => p.Name);

            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(p => p.ProductType)
                .IsRequired();

            builder.Property(p => p.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasOne(p => p.Asset)
                .WithMany(a => a.Products)
                .HasForeignKey(p => p.AssetId)
                .IsRequired();

            builder.HasMany(p => p.Layouts)
               .WithOne(l => l.Product)
               .IsRequired(false);
        }
    }
}
