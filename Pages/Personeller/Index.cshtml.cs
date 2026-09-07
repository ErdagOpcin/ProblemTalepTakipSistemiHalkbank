using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;
using ProblemTalepTakipSistemiHalkbank.Services;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    public class IndexModel : PageModel
    {
        // 1. İki bağımlılığı da (Veritabanı ve Şehir API Servisi) tanımlıyoruz
        private readonly ApplicationDbContext _context;
        private readonly CityApiService _cityApiService;

        // 2. Constructor içinde ikisini birden sisteme enjekte ediyoruz
        public IndexModel(ApplicationDbContext context, CityApiService cityApiService)
        {
            _context = context;
            _cityApiService = cityApiService;
        }

        // 3. Ön yüze aktarılacak veriler
        public IList<Personel> PersonelListesi { get; set; } = default!;
        public List<CityDto> Cities { get; set; } = new();

        // 4. Sayfa açıldığında ikisini de asenkron olarak dolduruyoruz
        public async Task OnGetAsync()
        {
            // Veritabanından personelleri çek
            PersonelListesi = await _context.Personeller.ToListAsync();

            // Harici API'den illeri çek
            Cities = await _cityApiService.GetCitiesAsync();
        }
    }
}