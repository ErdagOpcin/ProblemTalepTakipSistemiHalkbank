using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // TABLOLAR
        // =====================================================

        public DbSet<Personel> Personeller { get; set; }

        public DbSet<Problem> Problemler { get; set; }

        public DbSet<ProblemPersonel> ProblemPersoneller { get; set; }

        public DbSet<PersonelTask> PersonelTasklari { get; set; }

        public DbSet<Bildirim> Bildirimler { get; set; }


        // =====================================================
        // MODEL İLİŞKİLERİ
        // =====================================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =================================================
            // PROBLEM - PERSONEL MANY-TO-MANY
            // =================================================

            modelBuilder.Entity<ProblemPersonel>()
                .HasKey(pp => new
                {
                    pp.ProblemId,
                    pp.PersonelId
                });


            modelBuilder.Entity<ProblemPersonel>()
                .HasOne(pp => pp.Problem)
                .WithMany(p => p.ProblemPersoneller)
                .HasForeignKey(pp => pp.ProblemId);


            modelBuilder.Entity<ProblemPersonel>()
                .HasOne(pp => pp.Personel)
                .WithMany(p => p.ProblemPersoneller)
                .HasForeignKey(pp => pp.PersonelId);


            // =================================================
            // PERSONEL - TASK
            // =================================================

            modelBuilder.Entity<PersonelTask>()
                .HasOne(t => t.Personel)
                .WithMany(p => p.PersonelTasklari)
                .HasForeignKey(t => t.PersonelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}