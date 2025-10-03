using Microsoft.EntityFrameworkCore;
using TicketBeasts.Models;

namespace TicketBeasts.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets tell EF to make tables for these models
        public DbSet<Sport> Sports { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Owner> Owners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}