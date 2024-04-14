using Domain.Models;
using Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Context
{
    public class SqlContext(DbContextOptions<SqlContext> opt) : DbContext(opt)
    {
        internal DbSet<MachineSchedule> MachineShift { get; set; }
        internal DbSet<ScheduledStop> ScheduledStop { get; set; }
        internal DbSet<Shift> Shift { get; set; }
        internal DbSet<EggCategory> EggCategory { get; set; }
        internal DbSet<User> User { get; set; }
        internal DbSet<Customer> Customer { get; set; }
        internal DbSet<Plant> Plant { get; set; }
        internal DbSet<Asset> Asset { get; set; }
        internal DbSet<Product> Product { get; set; }
        internal DbSet<Layout> Layout { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MachineScheduleMapping());
            builder.ApplyConfiguration(new ScheduledStopMapping());
            builder.ApplyConfiguration(new ShiftMapping());
            builder.ApplyConfiguration(new EggCategoryMapping());
            builder.ApplyConfiguration(new UserMapping());
            builder.ApplyConfiguration(new CustomerMapping());
            builder.ApplyConfiguration(new PlantMapping());
            builder.ApplyConfiguration(new AssetMapping());
            builder.ApplyConfiguration(new ProductMapping());
            builder.ApplyConfiguration(new LayoutMapping());

            base.OnModelCreating(builder);
        }
    }
}
