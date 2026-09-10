using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
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
        public Personel Personel { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Sayfa ilk açıldığında boş formu ekrana basar
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Model kuralları doğrulanmadıysa sayfayı formdaki hatalarla yeniden göster
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Formdan gelen personeli veritabanına ekle
            _context.Personeller.Add(Personel);
            await _context.SaveChangesAsync();

            // Kayıt bitince personel listesi sayfasına geri dön
            return RedirectToPage("./Index");
        }
    }
}