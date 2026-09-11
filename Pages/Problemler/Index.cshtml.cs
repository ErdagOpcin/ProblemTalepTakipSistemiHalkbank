using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Problemler
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Problem> Problemler { get; set; }
            = new List<Problem>();

        // URL üzerinden seçilen durum bilgisini alır.
        [BindProperty(SupportsGet = true)]
        public ProblemDurumu? DurumFiltre { get; set; }
        [BindProperty(SupportsGet = true)]
        public ProblemOncelik? SecilenOncelik { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SecilenPersonelId { get; set;}
        public List<SelectListItem> PersonelListesi { get; set; } = new();
        public async Task OnGetAsync()
        {
            var personeller = await _context.Personeller
                .OrderBy(p => p.AdSoyad)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.AdSoyad
                })
                .ToListAsync();
            personeller.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = "⚠️ Atanmadı"
            });
            PersonelListesi = personeller;
            // Problem ve atanmış tüm personellerin bilgisini birlikte getir.
            IQueryable<Problem> query = _context.Problemler
                .Include(p => p.ProblemPersoneller)
                    .ThenInclude(pp => pp.Personel);

            // Admin olmayan kullanıcı yalnızca
            // kendisine atanmış problemleri görebilir.
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = _userManager.GetUserId(User);

                query = query.Where(p =>
                    p.ProblemPersoneller.Any(pp => pp.Personel.IdentityUserId == currentUserId));
            }

            // Kullanıcı bir durum seçtiyse
            // yalnızca o durumdaki problemleri getir.
            if (DurumFiltre.HasValue)
            {
                query = query.Where(p =>
                    p.Durum == DurumFiltre.Value);
            }
            if(SecilenOncelik.HasValue)
            {
                query = query.Where(p=>p.Oncelik == SecilenOncelik.Value);
            }

            // SEÇİLEN PERSONEL FİLTRESİ
            if (SecilenPersonelId.HasValue)
            {
                if (SecilenPersonelId.Value == -1)
                {
                    // Atanmadı seçildiyse: ProblemPersoneller listesi boş olanları getir
                    query = query.Where(p => !p.ProblemPersoneller.Any());
                }
                else
                {
                    // Normal personel seçildiyse: O personelin ID'sini ara
                    query = query.Where(p => p.ProblemPersoneller.Any(pp => pp.PersonelId == SecilenPersonelId.Value));
                }
            }

            // En yeni problemler üstte gösterilir.
            Problemler = await query
                .OrderByDescending(p => p.OlusturulmaTarihi)
                .ToListAsync();
        }
    }
}