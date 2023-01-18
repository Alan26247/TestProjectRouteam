using BillingService.Services;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Owner> Owners { get; set; }



        private readonly string connectionString;

        public AppDbContext(IConnectionString connectionString)
        {
            this.connectionString = connectionString.ConnectionString;

            //Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}