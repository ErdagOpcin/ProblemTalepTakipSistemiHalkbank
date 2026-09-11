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

    // PROBLEM İSTATİSTİKLERİ
    public int ToplamProblem { get; set; }
    public int BekleyenProblem { get; set; }
    public int IslemdeProblem { get; set; }
    public int CozulenProblem { get; set; }

    public List<Problem> SonProblemler { get; set; }
        = new List<Problem>();

    // TASK İSTATİSTİKLERİ
    public int ToplamTask { get; set; }
    public int BekleyenTask { get; set; }
    public int DevamEdenTask { get; set; }
    public int TamamlananTask { get; set; }

    public decimal ToplamPlanlananEfor { get; set; }
    public decimal ToplamHarcananEfor { get; set; }

    public List<PersonelTask> SonTasklar { get; set; }
        = new List<PersonelTask>();

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

        // =====================================================
        // PROBLEMLER
        // =====================================================

        IQueryable<Problem> problemQuery = _context.Problemler
            .Include(p => p.ProblemPersoneller)
                .ThenInclude(pp => pp.Personel);

        // Personel sadece kendisine atanmış problemleri görür.
        // Admin bütün problemleri görür.
        if (!User.IsInRole("Admin"))
        {
            problemQuery = problemQuery.Where(p =>
                p.ProblemPersoneller.Any(
                    pp => pp.Personel.IdentityUserId == user.Id
                )
            );
        }

        ToplamProblem = await problemQuery.CountAsync();

        BekleyenProblem = await problemQuery.CountAsync(
            p => p.Durum == ProblemDurumu.Bekliyor
        );

        IslemdeProblem = await problemQuery.CountAsync(
            p => p.Durum == ProblemDurumu.IslemeAlindi
        );

        CozulenProblem = await problemQuery.CountAsync(
            p => p.Durum == ProblemDurumu.Cozuldu
        );

        SonProblemler = await problemQuery
            .OrderByDescending(p => p.OlusturulmaTarihi)
            .Take(5)
            .ToListAsync();


        // =====================================================
        // TASKLAR
        // =====================================================

        IQueryable<PersonelTask> taskQuery = _context.PersonelTasklari
            .Include(t => t.Personel);

        // Personel sadece kendisine atanmış taskları görür.
        // Admin bütün taskları görür.
        if (!User.IsInRole("Admin"))
        {
            taskQuery = taskQuery.Where(t =>
                t.Personel.IdentityUserId == user.Id
            );
        }

        ToplamTask = await taskQuery.CountAsync();

        BekleyenTask = await taskQuery.CountAsync(
            t => t.Durum == TaskDurumu.Bekliyor
        );

        DevamEdenTask = await taskQuery.CountAsync(
            t => t.Durum == TaskDurumu.DevamEdiyor
        );

        TamamlananTask = await taskQuery.CountAsync(
            t => t.Durum == TaskDurumu.Tamamlandi
        );

        ToplamPlanlananEfor = await taskQuery
            .SumAsync(t => (decimal?)t.PlanlananEfor)
            ?? 0;

        ToplamHarcananEfor = await taskQuery
            .SumAsync(t => t.HarcananEfor)
            ?? 0;

        SonTasklar = await taskQuery
            .OrderByDescending(t => t.OlusturulmaTarihi)
            .Take(5)
            .ToListAsync();
    }
}