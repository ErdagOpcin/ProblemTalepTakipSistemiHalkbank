using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Personel Personel { get; set; } = default!;

        // 1. GET İsteği: Onay sayfasını açar, silinecek personeli gösterir
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller.FirstOrDefaultAsync(m => m.Id == id);

            if (personel == null)
            {
                return NotFound();
            }

            Personel = personel;
            return Page();
        }

        // 2. POST İsteği: Kullanıcı "Sil" butonuna bastığında asıl silme işini yapar
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personeller.FindAsync(id);

            if (personel != null)
            {
                Personel = personel;
                _context.Personeller.Remove(Personel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}