using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SLPDBLibrary
{
    public class DatabaseContext:DbContext
    {
        public DbSet<tbBranch> tbBranch { get; set; }
        public DbSet<tbCities> tbCities { get; set; }
        public DbSet<tbRegions> tbRegions { get; set; }
        public DbSet<tbMeters> tbMeters { get; set; }
        public DatabaseContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=EcoStruxture;Username=admin;Password=srV0rl@nd");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}