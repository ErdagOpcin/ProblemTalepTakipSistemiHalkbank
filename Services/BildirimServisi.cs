using System.Collections.Generic;
using System.Threading.Tasks;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public class BildirimServisi : IBildirimServisi
    {
        private readonly ApplicationDbContext _context;

        public BildirimServisi(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task BildirimGonderAsync(int personelId, int problemId, string baslik, string mesaj)
        {
            var bildirim = new Bildirim
            {
                PersonelId = personelId,
                ProblemId = problemId,
                Baslik = baslik,
                Mesaj = mesaj,
                OlusturulmaTarihi = DateTime.Now,
                OkunduMu = false
            };

            _context.Bildirimler.Add(bildirim);
            await _context.SaveChangesAsync();
        }

        public async Task TopluBildirimGonderAsync(IEnumerable<int> personelIdler, int problemId, string baslik, string mesaj)
        {
            foreach (var pId in personelIdler)
            {
                _context.Bildirimler.Add(new Bildirim
                {
                    PersonelId = pId,
                    ProblemId = problemId,
                    Baslik = baslik,
                    Mesaj = mesaj,
                    OlusturulmaTarihi = DateTime.Now,
                    OkunduMu = false
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}