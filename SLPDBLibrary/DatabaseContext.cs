using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SLPDBLibrary
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

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
        public DbSet<tbEmployees> tbEmployees { get; set; }
        public DbSet<trend_data> trend_datas { get; set; }
        public DbSet<trend_meta> trend_Metas { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var configSection = configBuilder.GetSection("DatabaseSettings");
            var connectionString = configSection["ConnectionStrings"] ?? null;

            NpgsqlConnectionStringBuilder connString = new NpgsqlConnectionStringBuilder(connectionString);
            

            if (connectionString != null)
            {
                optionsBuilder.UseNpgsql(connectionString);
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<trend_data>()
                .HasKey(e => e.originalseqno);

            modelBuilder.Entity<trend_meta>()
                .HasKey(e => e.originatedguid);

        }
    }
}