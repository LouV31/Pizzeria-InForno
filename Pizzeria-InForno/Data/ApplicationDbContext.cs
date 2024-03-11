using Microsoft.EntityFrameworkCore;
using Pizzeria_InForno.Models;

namespace Pizzeria_InForno.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Articoli> Articoli { get; set; }
        public DbSet<DettagliOrdine> DettagliOrdine { get; set; }
        public DbSet<Ordini> Ordini { get; set; }
        public DbSet<Utenti> Utenti { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Utenti>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
