using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;
using ProblemTalepTakipSistemiHalkbank.Services;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Problemler
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly CityApiService _cityApiService;

        public CreateModel(
            ApplicationDbContext context,
            CityApiService cityApiService)
        {
            _context = context;
            _cityApiService = cityApiService;
        }


        [BindProperty]
        public Problem Problem { get; set; } = default!;


        public List<SelectListItem> SehirlerListesi { get; set; } = new();


        public List<SelectListItem> PersonelListesi { get; set; } = new();


        public async Task<IActionResult> OnGetAsync()
        {
            await DropdownListeleriniDoldur();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await DropdownListeleriniDoldur();

                return Page();
            }


            Problem.OlusturulmaTarihi = DateTime.Now;


            _context.Problemler.Add(Problem);


            await _context.SaveChangesAsync();


            return RedirectToPage("./Index");
        }


        private async Task DropdownListeleriniDoldur()
        {
            var sehirler =
                await _cityApiService.GetCitiesAsync();


            SehirlerListesi = sehirler
                .Select(s => new SelectListItem
                {
                    Text = s.sehirIsmi,
                    Value = s.sehirIsmi
                })
                .ToList();


            var personeller =
                await _context.Personeller
                    .OrderBy(p => p.AdSoyad)
                    .ToListAsync();


            PersonelListesi = personeller
                .Select(p => new SelectListItem
                {
                    Text = $"{p.AdSoyad} - {p.Departman}",
                    Value = p.Id.ToString()
                })
                .ToList();
        }
    }
}