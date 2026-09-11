using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Bildirimler
{
    [Authorize]
    [IgnoreAntiforgeryToken] // AJAX çağrılarının takılmaması için
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetBildirimlerAsync()
        {
            var userId = _userManager.GetUserId(User);
            
            // Giriş yapan personeli bul
            var personel = await _context.Personeller.FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (personel == null)
            {
                return new JsonResult(new { sayi = 0, bildirimler = new List<object>() });
            }

            var bildirimler = await _context.Bildirimler
                .Where(b => b.PersonelId == personel.Id && !b.OkunduMu)
                .OrderByDescending(b => b.OlusturulmaTarihi)
                .Take(10)
                .Select(b => new
                {
                    id = b.Id,
                    baslik = b.Baslik,
                    mesaj = b.Mesaj,
                    problemId = b.ProblemId,
                    tarih = b.OlusturulmaTarihi.ToString("dd.MM.yyyy HH:mm")
                })
                .ToListAsync();

            return new JsonResult(new { sayi = bildirimler.Count, bildirimler });
        }

        public async Task<IActionResult> OnPostTumunuOkuAsync()
        {
            var userId = _userManager.GetUserId(User);
            var personel = await _context.Personeller.FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (personel != null)
            {
                var okunmamislar = await _context.Bildirimler
                    .Where(b => b.PersonelId == personel.Id && !b.OkunduMu)
                    .ToListAsync();

                foreach (var b in okunmamislar)
                {
                    b.OkunduMu = true;
                }

                await _context.SaveChangesAsync();
            }

            return new JsonResult(new { success = true });
        }
    }
}