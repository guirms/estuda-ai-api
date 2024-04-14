using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers")
                .HasIndex(c => new { c.BatchStatus, c.Name });

            builder.HasKey(c => c.CustomerId);

            builder.Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Cnpj)
                .HasColumnType("char(14)")
                .IsRequired();

            builder.Property(c => c.BatchStatus)
                .IsRequired();

            builder.Property(c => c.ProcessedBatches)
                .IsRequired();

            builder.Property(c => c.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();
        }
    }
}
