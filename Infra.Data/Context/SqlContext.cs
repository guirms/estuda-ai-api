using Domain.Models;
using Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Context
{
    public class SqlContext(DbContextOptions<SqlContext> opt) : DbContext(opt)
    {
        internal DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserMapping());

            base.OnModelCreating(builder);
        }
    }
}
