using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Mappings
{
    public class ScheduledStopMapping : IEntityTypeConfiguration<ScheduledStop>
    {
        public void Configure(EntityTypeBuilder<ScheduledStop> builder)
        {
            builder.ToTable("ScheduledStops");

            builder.HasKey(s => s.ScheduledStopId);

            builder.Property(s => s.Name)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(s => s.StartTime)
                .IsRequired()
                .HasColumnType("time(0)");

            builder.Property(s => s.EndTime)
                .IsRequired()
                .HasColumnType("time(0)");

            builder.Property(s => s.InsertedAt)
                .HasColumnType("datetime(0)");

            builder.HasOne(m => m.MachineSchedule)
                .WithMany(m => m.ScheduledStops)
                .HasForeignKey(s => s.MachineScheduleId)
                .IsRequired();
        }
    }
}
