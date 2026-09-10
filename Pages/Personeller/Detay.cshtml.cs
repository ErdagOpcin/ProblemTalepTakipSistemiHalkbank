using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    [Authorize]
    public class DetayModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetayModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Personel Personel { get; set; } = default!;

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
            return Page();
        }
    }
}