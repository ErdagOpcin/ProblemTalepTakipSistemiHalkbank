using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Pages;

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


    public Personel? GirisYapanPersonel { get; set; }


    public int ToplamProblem { get; set; }

    public int BekleyenProblem { get; set; }

    public int IslemdeProblem { get; set; }

    public int CozulenProblem { get; set; }


    public List<Problem> SonProblemler { get; set; }
        = new List<Problem>();


    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return;
        }


        GirisYapanPersonel = await _context.Personeller
            .FirstOrDefaultAsync(
                p => p.IdentityUserId == user.Id
            );


        IQueryable<Problem> query = _context.Problemler
            .Include(p => p.Personel);


        // Personel sadece kendisine atanmış
        // problemlerin istatistiklerini görür.
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(p =>
                p.Personel != null &&
                p.Personel.IdentityUserId == user.Id
            );
        }


        ToplamProblem = await query.CountAsync();


        BekleyenProblem = await query.CountAsync(
            p => p.Durum == ProblemDurumu.Bekliyor
        );


        IslemdeProblem = await query.CountAsync(
            p => p.Durum == ProblemDurumu.IslemeAlindi
        );


        CozulenProblem = await query.CountAsync(
            p => p.Durum == ProblemDurumu.Cozuldu
        );


        SonProblemler = await query
            .OrderByDescending(p => p.OlusturulmaTarihi)
            .Take(5)
            .ToListAsync();
    }
}