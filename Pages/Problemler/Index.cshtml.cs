using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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


        public async Task OnGetAsync()
        {
            // Önce Problem + Personel ilişkisini içeren sorguyu hazırlıyoruz.
            IQueryable<Problem> query = _context.Problemler
                .Include(p => p.Personel);


            // Kullanıcı Admin değilse sadece kendisine
            // atanmış problemleri görebilir.
            if (!User.IsInRole("Admin"))
            {
                var currentUserId =
                    _userManager.GetUserId(User);


                query = query.Where(p =>
                    p.Personel != null &&
                    p.Personel.IdentityUserId == currentUserId);
            }


            // En yeni problem en üstte olacak şekilde listele.
            Problemler = await query
                .OrderByDescending(p => p.OlusturulmaTarihi)
                .ToListAsync();
        }
    }
}