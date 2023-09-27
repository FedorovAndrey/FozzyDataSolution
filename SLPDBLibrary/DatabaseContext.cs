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
        public DbSet<tbBranchSquare> tbBranchSquare { get; set; }
        public DbSet<tbCategory> tbCategory { get; set; }
        public DbSet<tbClimate> tbClimates { get; set; }
        public DbSet<tbMeterRoles> tbMeterRoles { get; set; }
        public DbSet<tbTradingSquare> tbTradingSquare { get; set; }
        public DbSet<tbTypeOfHeating> tbTypeOfHeatings { get; set; }
        public DbSet<tbMeterType> tbMeterTypes { get; set; }

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