using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class ShiftMapping : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable("Shifts")
                .HasIndex(s => s.Type);

            builder.HasKey(s => s.ShiftId);

            builder.Property(s => s.Type)
                .IsRequired();

            builder.Property(s => s.StartTime)
                .IsRequired()
                .HasColumnType("time(0)");

            builder.Property(s => s.EndTime)
                .IsRequired()
                .HasColumnType("time(0)");

            builder.Property(s => s.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.HasOne(m => m.MachineSchedule)
                .WithMany(m => m.Shifts)
                .HasForeignKey(s => s.MachineScheduleId)
                .IsRequired();
        }
    }
}
