using Microsoft.AspNetCore.Authorization;
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

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
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
                .Include(p => p.Problemler)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            Personel = personel;

            // Bir Identity kullanıcısına bağlı personel silinemez.
            if (!string.IsNullOrWhiteSpace(personel.IdentityUserId))
            {
                HataMesaji =
                    "Bu personel bir kullanıcı hesabına bağlı olduğu için silinemez.";

                return Page();
            }

            // Üzerinde atanmış problem olan personel silinemez.
            if (personel.Problemler.Any())
            {
                HataMesaji =
                    "Bu personele atanmış problemler bulunduğu için silinemez.";

                return Page();
            }

            _context.Personeller.Remove(personel);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}