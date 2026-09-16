using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Problemler
{
    [Authorize]
    public class DetayModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetayModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .FirstOrDefaultAsync(p => p.Id == id);

            if (problem == null)
            {
                return NotFound();
            }

            // Admin tüm problemleri görebilir.
            // Personel sadece kendisine atanmış problemi görebilir.
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = _userManager.GetUserId(User);

                var personeleAtanmisMi =
                    problem.ProblemPersoneller.Any(pp =>
                        pp.Personel != null &&
                        pp.Personel.IdentityUserId == currentUserId
                    );

                if (!personeleAtanmisMi)
                {
                    return Forbid();
                }
            }

            Problem = problem;

            return Page();
        }
    }
}