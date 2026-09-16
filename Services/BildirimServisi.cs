using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;

namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public class BildirimServisi : IBildirimServisi
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailServisi _emailServisi;

        public BildirimServisi(ApplicationDbContext context, IEmailServisi emailServisi)
        {
            _context = context;
            _emailServisi = emailServisi;
        }

        public async Task BildirimGonderAsync(int personelId, int problemId, string baslik, string mesaj)
        {
            // 1. Veritabanına uygulama içi bildirim kaydı
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

            // 2. Personelin e-posta adresine mail gönderimi
            var personel = await _context.Personeller.FirstOrDefaultAsync(p => p.Id == personelId);
            if (personel != null && !string.IsNullOrWhiteSpace(personel.Email))
            {
                await MailGonderGuvenliAsync(personel.Email, personel.AdSoyad, problemId, baslik, mesaj);
            }
        }

        public async Task TopluBildirimGonderAsync(IEnumerable<int> personelIdler, int problemId, string baslik, string mesaj)
        {
            var idList = personelIdler.Distinct().ToList();
            if (!idList.Any()) return;

            // 1. Veritabanına toplu bildirim kaydı
            foreach (var pId in idList)
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

            // 2. İlgili personellerin e-postalarını çekip her birine mail atma
            var personeller = await _context.Personeller
                .Where(p => idList.Contains(p.Id) && !string.IsNullOrWhiteSpace(p.Email))
                .ToListAsync();

            foreach (var personel in personeller)
            {
                await MailGonderGuvenliAsync(personel.Email, personel.AdSoyad, problemId, baslik, mesaj);
            }
        }

        // Mail hatası oluşursa ana akışın/veritabanı işleminin çökmesini engelleyen yardımcı metot
        private async Task MailGonderGuvenliAsync(string email, string adSoyad, int problemId, string baslik, string mesaj)
        {
            try
            {
                var emailKonu = $"[Halkbank Talep Takip] {baslik} (#{problemId})";
                var emailGovde = $@"
                    <div style=""font-family: 'Segoe UI', Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden;"">
                        <div style=""background: #003366; color: white; padding: 18px 24px;"">
                            <h3 style=""margin: 0; font-size: 18px;"">Halkbank Problem & Talep Takip</h3>
                        </div>
                        <div style=""padding: 24px; color: #1e293b;"">
                            <p style=""font-size: 15px; margin-top: 0;"">Sayın <strong>{adSoyad}</strong>,</p>
                            <div style=""background-color: #f8fafc; border-left: 4px solid #0056b3; padding: 14px; margin: 18px 0; border-radius: 4px;"">
                                <strong style=""display: block; color: #0056b3; margin-bottom: 4px;"">{baslik} (Talep #{problemId})</strong>
                                <span style=""font-size: 14px; color: #334155; line-height: 1.5;"">{mesaj}</span>
                            </div>
                            <p style=""font-size: 13px; color: #64748b; margin-bottom: 0;"">Detayları incelemek ve işlem yapmak için sisteme giriş yapabilirsiniz.</p>
                        </div>
                        <div style=""background: #f1f5f9; padding: 12px 24px; font-size: 12px; color: #94a3b8; text-align: center; border-top: 1px solid #e2e8f0;"">
                            Bu e-posta Halkbank İç Sistemleri tarafından otomatik olarak oluşturulmuştur.
                        </div>
                    </div>";

                await _emailServisi.SendEmailAsync(email, emailKonu, emailGovde);
            }
            catch (Exception)
            {
                // SMTP sunucusunda anlık kesinti olsa bile arayüzün patlamasını engeller
            }
        }
    }
}