using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class AssetMapping : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ToTable("Assets")
                .HasIndex(a => new { a.Name });

            builder.HasKey(a => a.AssetId);

            builder.Property(a => a.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(a => a.Ip)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(a => a.EggPackerQuantity)
                .IsRequired();

            builder.Property(a => a.DenesterQuantity)
                .IsRequired();

            builder.Property(a => a.HasFeedback)
                .IsRequired();

            builder.Property(a => a.Key)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(a => a.AuthToken)
                .HasMaxLength(300)
                .IsRequired(false);

            builder.Property(a => a.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(a => a.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasOne(a => a.Plant)
                .WithMany(p => p.Assets)
                .HasForeignKey(a => a.PlantId)
                .IsRequired();

            builder.HasMany(a => a.Users)
                .WithOne(u => u.Asset)
                .IsRequired(false);

            builder.HasMany(a => a.Products)
               .WithOne(p => p.Asset)
               .IsRequired(false);
        }
    }
}
