using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Bildirimler
{
    [Authorize]
    public class TumuModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TumuModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Bildirim> Bildirimler { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            var personel = await _context.Personeller.FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (personel == null)
            {
                return Page();
            }

            Bildirimler = await _context.Bildirimler
                .Include(b => b.Problem)
                .Where(b => b.PersonelId == personel.Id)
                .OrderByDescending(b => b.OlusturulmaTarihi)
                .ToListAsync();

            return Page();
        }

        // Tek bir bildirimi okundu yapıp probleme yönlendir
        public async Task<IActionResult> OnGetOkuVeGitAsync(int id)
        {
            var bildirim = await _context.Bildirimler.FindAsync(id);
            if (bildirim != null)
            {
                bildirim.OkunduMu = true;
                await _context.SaveChangesAsync();
                return RedirectToPage("/Problemler/Details", new { id = bildirim.ProblemId });
            }

            return RedirectToPage("./Tumu");
        }

        // Tüm bildirimleri tek tıkla okundu yap
        public async Task<IActionResult> OnPostHepsiniOkunduYapAsync()
        {
            var userId = _userManager.GetUserId(User);
            var personel = await _context.Personeller.FirstOrDefaultAsync(p => p.IdentityUserId == userId);

            if (personel != null)
            {
                var okunmamislar = await _context.Bildirimler
                    .Where(b => b.PersonelId == personel.Id && !b.OkunduMu)
                    .ToListAsync();

                foreach (var item in okunmamislar)
                {
                    item.OkunduMu = true;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Tumu");
        }
    }
}