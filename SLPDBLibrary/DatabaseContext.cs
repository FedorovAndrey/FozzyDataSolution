using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SLPDBLibrary
{
    public class DatabaseContext:DbContext
    {
        public DbSet<tbBranche> tbBranches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=EcoStruxture;Username=admin;Password=srV0rl@nd");
        }
    }
}