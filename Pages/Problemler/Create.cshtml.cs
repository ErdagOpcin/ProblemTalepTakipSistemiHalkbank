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
        public Problem Problem { get; set; } = new();

        [BindProperty]
        public int? SelectedSehirId { get; set; }

        // Formdan seçilen çoklu personel ID'leri
        [BindProperty]
        public List<int> SecilenPersonelIds { get; set; } = new();

        public List<SelectListItem> SehirlerListesi { get; set; } = new();
        public List<SelectListItem> IlcelerListesi { get; set; } = new();
        public List<SelectListItem> PersonelListesi { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await DropdownListeleriniDoldur();
            return Page();
        }

        public async Task<JsonResult> OnGetIlcelerAsync(int sehirId)
        {
            var ilceler = await _cityApiService.GetDistrictsAsync(sehirId);

            var sonuc = ilceler
                .Select(i => new
                {
                    id = i.Id,
                    name = i.ilceIsmi
                })
                .ToList();

            return new JsonResult(sonuc);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedSehirId.HasValue)
            {
                var sehirler = await _cityApiService.GetCitiesAsync();
                var secilenSehir = sehirler.FirstOrDefault(s => s.Id == SelectedSehirId.Value);

                if (secilenSehir != null)
                {
                    Problem.Sehir = secilenSehir.sehirIsmi;
                    ModelState.Remove("Problem.Sehir");
                }
            }

            // Formdan doğrudan doldurulmayan navigasyon özelliğinin doğrulamasını kaldır
            ModelState.Remove("Problem.ProblemPersoneller");

            if (!ModelState.IsValid)
            {
                await DropdownListeleriniDoldur();
                return Page();
            }

            Problem.OlusturulmaTarihi = DateTime.Now;
            Problem.Durum = ProblemDurumu.Bekliyor;

            // Seçilen personelleri ara tabloya bağla
            if (SecilenPersonelIds.Any())
            {
                foreach (var personelId in SecilenPersonelIds)
                {
                    Problem.ProblemPersoneller.Add(new ProblemPersonel
                    {
                        PersonelId = personelId
                    });
                }
            }

            _context.Problemler.Add(Problem);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task DropdownListeleriniDoldur()
        {
            var sehirler = await _cityApiService.GetCitiesAsync();

            SehirlerListesi = sehirler
                .Select(s => new SelectListItem
                {
                    Text = s.sehirIsmi,
                    Value = s.Id.ToString()
                })
                .ToList();

            if (SelectedSehirId.HasValue)
            {
                var ilceler = await _cityApiService.GetDistrictsAsync(SelectedSehirId.Value);

                IlcelerListesi = ilceler
                    .Select(i => new SelectListItem
                    {
                        Text = i.ilceIsmi,
                        Value = i.ilceIsmi
                    })
                    .ToList();
            }

            var personeller = await _context.Personeller
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