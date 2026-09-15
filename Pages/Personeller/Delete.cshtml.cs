using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Personel Personel { get; set; } = default!;

        public string? HataMesaji { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            Personel = personel;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller
                .Include(p => p.ProblemPersoneller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            Personel = personel;

            // 1. Üzerinde aktif problem varsa problem atamalarını tablodan kaldır (foreign key hatası vermesin)
            if (personel.ProblemPersoneller.Any())
            {
                _context.ProblemPersoneller.RemoveRange(personel.ProblemPersoneller);
            }

            // 2. Personele ait bildirimleri temizle
            var bildirimler = _context.Bildirimler.Where(b => b.PersonelId == personel.Id);
            _context.Bildirimler.RemoveRange(bildirimler);

            // 3. Bağlı Identity hesabını (AspNetUsers) sil
            if (!string.IsNullOrWhiteSpace(personel.IdentityUserId))
            {
                var user = await _userManager.FindByIdAsync(personel.IdentityUserId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            // 4. Personel kaydını sil
            _context.Personeller.Remove(personel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}