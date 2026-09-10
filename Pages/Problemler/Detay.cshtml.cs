using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Problemler
{
    public class DetayModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetayModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Problem Problem { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problemler
                .Include(p => p.ProblemPersoneller)
                .ThenInclude(pp => pp.Personel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (problem == null)
            {
                return NotFound();
            }

            // Personel sadece kendi görevine bakabilsin (Admin hepsini görebilir)
            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Giriş yapan kullanıcının Id'si bu problemin atanmış personelleri arasında YOKSA erişimi engelle
                if (!problem.ProblemPersoneller.Any(pp => pp.Personel.IdentityUserId == userId))
                {
                    return Forbid();
                }
            }

            Problem = problem;
            return Page();
        }
    }
}