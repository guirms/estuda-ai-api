using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class MachineScheduleMapping : IEntityTypeConfiguration<MachineSchedule>
    {
        public void Configure(EntityTypeBuilder<MachineSchedule> builder)
        {
            builder.ToTable("MachineSchedules")
                .HasIndex(m => new { m.WeekDay, m.InitialProductionTime, m.FinalProductionTime });

            builder.HasKey(m => m.MachineScheduleId);

            builder.Property(m => m.WeekDay)
                .IsRequired();

            builder.Property(m => m.InitialProductionTime)
                .HasColumnType("time(0)")
                .IsRequired();

            builder.Property(m => m.FinalProductionTime)
                .HasColumnType("time(0)")
                .IsRequired();

            builder.Property(m => m.InsertedAt)
                .HasColumnType("datetime(0)")
                .IsRequired();

            builder.Property(m => m.UpdatedAt)
                .HasColumnType("datetime(0)")
                .IsRequired(false);

            builder.HasMany(m => m.Shifts)
               .WithOne(s => s.MachineSchedule)
               .IsRequired(false);

            builder.HasMany(m => m.ScheduledStops)
               .WithOne(s => s.MachineSchedule)
               .IsRequired(false);

            builder.HasOne(m => m.User)
               .WithMany(u => u.MachineSchedules)
               .HasForeignKey(m => m.UserId)
               .IsRequired();
        }
    }
}
