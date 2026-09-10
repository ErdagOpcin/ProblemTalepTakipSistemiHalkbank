using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Problemler
{
    [Authorize(Roles = "Admin")]
    public class DurumGuncelleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DurumGuncelleModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Problem Problem { get; set; } = default!;

        [BindProperty]
        public List<int> SecilenPersonelIds { get; set; } = new();

        public MultiSelectList PersonelListesi { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problemler
                .Include(p => p.ProblemPersoneller)
                    .ThenInclude(pp => pp.Personel)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (problem == null)
            {
                return NotFound();
            }

            Problem = problem;
            SecilenPersonelIds = Problem.ProblemPersoneller.Select(pp => pp.PersonelId).ToList();

            await PersonelListesiniYukleAsync(SecilenPersonelIds);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var mevcutProblem = await _context.Problemler
                .Include(p => p.ProblemPersoneller)
                .FirstOrDefaultAsync(p => p.Id == Problem.Id);

            if (mevcutProblem == null)
            {
                return NotFound();
            }

            // Problem durumunu güncelle
            mevcutProblem.Durum = Problem.Durum;

            if (Problem.Durum == ProblemDurumu.Cozuldu)
            {
                if (mevcutProblem.CozulmeTarihi == null)
                {
                    mevcutProblem.CozulmeTarihi = DateTime.Now;
                }
            }
            else
            {
                mevcutProblem.CozulmeTarihi = null;
            }

            if (!string.IsNullOrWhiteSpace(Problem.Baslik))
            {
                mevcutProblem.Baslik = Problem.Baslik;
            }

            mevcutProblem.Aciklama = Problem.Aciklama;
            mevcutProblem.Oncelik = Problem.Oncelik;

            // Çoklu Personel Güncelleme (Eski kayıtları temizleyip yenilerini ekliyoruz)
            mevcutProblem.ProblemPersoneller.Clear();

            if (SecilenPersonelIds != null && SecilenPersonelIds.Any())
            {
                foreach (var personelId in SecilenPersonelIds)
                {
                    mevcutProblem.ProblemPersoneller.Add(new ProblemPersonel
                    {
                        ProblemId = mevcutProblem.Id,
                        PersonelId = personelId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task PersonelListesiniYukleAsync(List<int> seciliIds)
        {
            var personeller = await _context.Personeller
                .Select(p => new { p.Id, p.AdSoyad })
                .ToListAsync();

            PersonelListesi = new MultiSelectList(personeller, "Id", "AdSoyad", seciliIds);
        }
    }
}