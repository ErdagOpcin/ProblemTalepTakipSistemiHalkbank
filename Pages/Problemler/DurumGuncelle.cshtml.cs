using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problemler
                .Include(p => p.Personel)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (problem == null)
            {
                return NotFound();
            }

            Problem = problem;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var mevcutProblem = await _context.Problemler
                .FirstOrDefaultAsync(p => p.Id == Problem.Id);

            if (mevcutProblem == null)
            {
                return NotFound();
            }

            mevcutProblem.Durum = Problem.Durum;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}