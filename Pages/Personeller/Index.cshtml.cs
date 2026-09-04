using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Personel> PersonelListesi { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Veritabanındaki tüm personelleri asenkron olarak listeliyoruz
            PersonelListesi = await _context.Personeller.ToListAsync();
        }
    }
}