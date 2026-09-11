using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Personel> Personeller { get; set; }
        public DbSet<Problem> Problemler { get; set; }
        public DbSet<ProblemPersonel> ProblemPersoneller { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ara tablonun bileşik birincil anahtarı (Composite Key)
            modelBuilder.Entity<ProblemPersonel>()
                .HasKey(pp => new { pp.ProblemId, pp.PersonelId });

            // Problem - ProblemPersonel ilişkisi
            modelBuilder.Entity<ProblemPersonel>()
                .HasOne(pp => pp.Problem)
                .WithMany(p => p.ProblemPersoneller)
                .HasForeignKey(pp => pp.ProblemId);

            // Personel - ProblemPersonel ilişkisi
            modelBuilder.Entity<ProblemPersonel>()
                .HasOne(pp => pp.Personel)
                .WithMany(p => p.ProblemPersoneller)
                .HasForeignKey(pp => pp.PersonelId);
        }
    }
}