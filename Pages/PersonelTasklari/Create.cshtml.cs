using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.PersonelTasklari
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PersonelTask PersonelTask { get; set; } = new();

        public SelectList PersonelListesi { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await PersonelleriYukle();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PersonelleriYukle();
                return Page();
            }

            PersonelTask.OlusturulmaTarihi = DateTime.Now;
            PersonelTask.Durum = TaskDurumu.Bekliyor;

            _context.PersonelTasklari.Add(PersonelTask);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        private async Task PersonelleriYukle()
        {
            var personeller = await _context.Personeller
                .OrderBy(p => p.AdSoyad)
                .ToListAsync();

            PersonelListesi = new SelectList(
                personeller,
                "Id",
                "AdSoyad"
            );
        }
    }
}