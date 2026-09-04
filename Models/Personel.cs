using Microsoft.AspNetCore.Identity;

namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class Personel
    {
        public int Id { get; set; }

        public string AdSoyad { get; set; } = string.Empty;

        public string Departman { get; set; } = string.Empty;

        public string? IdentityUserId { get; set; }

        public IdentityUser? IdentityUser { get; set; }

        public List<Problem> Problemler { get; set; } = new List<Problem>();
    }
}