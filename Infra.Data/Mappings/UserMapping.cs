using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users")
                .HasIndex(u => new { u.Email, u.Name, u.Document });

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(89)
                .IsRequired();

            builder.Property(u => u.Document)
                .HasMaxLength(14)
                .IsRequired();

            builder.Property(u => u.Password)
                .HasColumnType("char(44)")
                .IsRequired();

            builder.Property(u => u.Salt)
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(u => u.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasMany(u => u.Boards)
                .WithOne(b => b.User)
                .IsRequired(false);
        }
    }
}
