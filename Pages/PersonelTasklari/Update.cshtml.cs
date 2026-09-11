using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.PersonelTasklari
{
    [Authorize(Roles = "Personel")]
    public class UpdateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UpdateModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public PersonelTask PersonelTask { get; set; } = default!;

        [BindProperty]
        public decimal? HarcananEfor { get; set; }

        [BindProperty]
        public TaskDurumu Durum { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            PersonelTask = await _context.PersonelTasklari
                .Include(t => t.Personel)
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.Personel != null &&
                    t.Personel.IdentityUserId == user.Id
                );

            if (PersonelTask == null)
            {
                return NotFound();
            }

            HarcananEfor = PersonelTask.HarcananEfor;
            Durum = PersonelTask.Durum;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var task = await _context.PersonelTasklari
                .Include(t => t.Personel)
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.Personel != null &&
                    t.Personel.IdentityUserId == user.Id
                );

            if (task == null)
            {
                return NotFound();
            }

            if (HarcananEfor.HasValue && HarcananEfor.Value < 0)
            {
                ModelState.AddModelError(
                    nameof(HarcananEfor),
                    "Harcanan efor 0'dan küçük olamaz."
                );
            }

            if (!ModelState.IsValid)
            {
                PersonelTask = task;
                return Page();
            }

            task.HarcananEfor = HarcananEfor;
            task.Durum = Durum;

            if (Durum == TaskDurumu.Tamamlandi)
            {
                task.TamamlanmaTarihi = DateTime.Now;
            }
            else
            {
                task.TamamlanmaTarihi = null;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}