using Domain.Models;
using Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Context
{
    public class SqlContext(DbContextOptions<SqlContext> opt) : DbContext(opt)
    {
        internal DbSet<User> User { get; set; }
        internal DbSet<Board> Board { get; set; }
        internal DbSet<Card> Card { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserMapping());
            builder.ApplyConfiguration(new BoardMapping());
            builder.ApplyConfiguration(new CardMapping());

            base.OnModelCreating(builder);
        }
    }
}
