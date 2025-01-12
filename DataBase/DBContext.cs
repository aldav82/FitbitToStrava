using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        public DbSet<Config> Configs { get; set; } // Represents the Users table
        public DbSet<Excercise> Excercises { get; set; } // Represents the Users table

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure the Name column is unique
            modelBuilder.Entity<Config>()
                .HasIndex(u => u.Name).IsUnique()
                .IsUnique();
            modelBuilder.Entity<Excercise>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
