using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class PlantMapping : IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.ToTable("Plants")
                .HasIndex(p => new { p.Name, p.Cnpj });

            builder.HasKey(p => p.PlantId);

            builder.Property(p => p.Name)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(p => p.Cnpj)
                .HasColumnType("char(14)")
                .IsRequired();

            builder.Property(p => p.Address)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(p => p.ZipCode)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(p => p.Latitude)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(p => p.Longitude)
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(p => p.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasMany(p => p.Assets)
               .WithOne(a => a.Plant)
               .IsRequired(false);

            builder.HasOne(p => p.User)
               .WithMany(u => u.Plants)
               .HasForeignKey(p => p.UserId)
               .IsRequired();
        }
    }
}
