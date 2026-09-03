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

        public DbSet<Problem> Problems { get; set; }
        public DbSet<Personel> Personeller { get; set; }
    }
}