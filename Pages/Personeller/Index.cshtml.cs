using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;
using ProblemTalepTakipSistemiHalkbank.Services;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly CityApiService _cityApiService;

        public IndexModel(
            ApplicationDbContext context,
            CityApiService cityApiService)
        {
            _context = context;
            _cityApiService = cityApiService;
        }

        public IList<Personel> PersonelListesi { get; set; } = default!;

        public List<CityDto> Cities { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Sadece Admin bu sayfaya erişebilir.

            PersonelListesi = await _context.Personeller
                .ToListAsync();

            Cities = await _cityApiService
                .GetCitiesAsync();
        }
    }
}