using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    [Authorize(Roles = "Admin")]
    public class DetayModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetayModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Personel Personel { get; set; } = default!;
        public int ToplamGorevSayisi { get; set; }
        public int TamamlananGorevSayisi { get; set; }
        public int DevamEdenGorevSayisi { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .Include(p => p.ProblemPersoneller)
                    .ThenInclude(pp => pp.Problem)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            Personel = personel;
            
            var atananProblemler = personel.ProblemPersoneller
                .Select(pp => pp.Problem)
                .Where(p => p != null)
                .ToList();
            ToplamGorevSayisi = atananProblemler.Count;
            TamamlananGorevSayisi = atananProblemler.Count(p => p.Durum == ProblemDurumu.Cozuldu);
            DevamEdenGorevSayisi = ToplamGorevSayisi - TamamlananGorevSayisi;
            return Page();
        }
    }
}