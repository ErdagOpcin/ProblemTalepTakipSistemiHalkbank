using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

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

        public async Task OnGetAsync()
        {
            // Problem ve atanmış personel bilgisini birlikte getir.
            IQueryable<Problem> query = _context.Problemler
                .Include(p => p.Personel);

            // Admin olmayan kullanıcı yalnızca
            // kendisine atanmış problemleri görebilir.
            if (!User.IsInRole("Admin"))
            {
                var currentUserId =
                    _userManager.GetUserId(User);

                query = query.Where(p =>
                    p.Personel != null &&
                    p.Personel.IdentityUserId == currentUserId);
            }

            // Kullanıcı bir durum seçtiyse
            // yalnızca o durumdaki problemleri getir.
            if (DurumFiltre.HasValue)
            {
                query = query.Where(p =>
                    p.Durum == DurumFiltre.Value);
            }

            // En yeni problemler üstte gösterilir.
            Problemler = await query
                .OrderByDescending(p => p.OlusturulmaTarihi)
                .ToListAsync();
        }
    }
}