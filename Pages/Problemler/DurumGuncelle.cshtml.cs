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
    public class DurumGuncelleModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IBildirimServisi _bildirimServisi;

        // 1. Servisi Constructor'a ekle:
        public DurumGuncelleModel(ApplicationDbContext context, IBildirimServisi bildirimServisi)
        {
            _context = context;
            _bildirimServisi = bildirimServisi;
        }
        [BindProperty]
        public Problem Problem { get; set; } = default!;

        [BindProperty]
        public List<int> SecilenPersonelIds { get; set; } = new();

        public MultiSelectList PersonelListesi { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problem = await _context.Problemler
                .Include(p => p.ProblemPersoneller)
                    .ThenInclude(pp => pp.Personel)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (problem == null)
            {
                return NotFound();
            }

            Problem = problem;
            SecilenPersonelIds = Problem.ProblemPersoneller.Select(pp => pp.PersonelId).ToList();

            await PersonelListesiniYukleAsync(SecilenPersonelIds);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var mevcutProblem = await _context.Problemler
                .Include(p => p.ProblemPersoneller)
                .FirstOrDefaultAsync(p => p.Id == Problem.Id);

            if (mevcutProblem == null)
            {
                return NotFound();
            }

            // 1. ESKİ DEĞERLERİ DEĞİŞİKLİKTEN ÖNCE SAKLA
            var eskiDurum = mevcutProblem.Durum;
            var eskiOncelik = mevcutProblem.Oncelik;
            var eskiPersonelIdler = mevcutProblem.ProblemPersoneller
                .Select(pp => pp.PersonelId)
                .ToList();

            var yeniPersonelIdler = SecilenPersonelIds ?? new List<int>();

            // 2. FARK ANALİZİ (Personel Atamaları)
            var eklenenPersoneller = yeniPersonelIdler.Except(eskiPersonelIdler).ToList();
            var cikarilanPersoneller = eskiPersonelIdler.Except(yeniPersonelIdler).ToList();
            var gorevdeKalanlar = eskiPersonelIdler.Intersect(yeniPersonelIdler).ToList();

            // 3. MODELİ GÜNCELLE
            mevcutProblem.Durum = Problem.Durum;
            mevcutProblem.Oncelik = Problem.Oncelik;
            mevcutProblem.Aciklama = Problem.Aciklama;

            if (!string.IsNullOrWhiteSpace(Problem.Baslik))
            {
                mevcutProblem.Baslik = Problem.Baslik;
            }

            if (Problem.Durum == ProblemDurumu.Cozuldu)
            {
                if (mevcutProblem.CozulmeTarihi == null)
                {
                    mevcutProblem.CozulmeTarihi = DateTime.Now;
                }
            }
            else
            {
                mevcutProblem.CozulmeTarihi = null;
            }

            // Personel listesini senkronize et
            mevcutProblem.ProblemPersoneller.Clear();
            foreach (var personelId in yeniPersonelIdler)
            {
                mevcutProblem.ProblemPersoneller.Add(new ProblemPersonel
                {
                    ProblemId = mevcutProblem.Id,
                    PersonelId = personelId
                });
            }

            // Veritabanına kaydet
            await _context.SaveChangesAsync();

            // 4. BİLDİRİM SENARYOLARI

            // Senaryo 1: Göreve yeni atanan kişiye bildirim
            foreach (var personelId in eklenenPersoneller)
            {
                await _bildirimServisi.BildirimGonderAsync(
                    personelId,
                    mevcutProblem.Id,
                    "Yeni Task Atandı",
                    $"'{mevcutProblem.Baslik}' başlıklı görev size atandı.");
            }

            // Senaryo 2: Taskta kalan mevcut kişilere "yeni kişi eklendi" bildirimi
            if (eklenenPersoneller.Any() && gorevdeKalanlar.Any())
            {
                await _bildirimServisi.TopluBildirimGonderAsync(
                    gorevdeKalanlar,
                    mevcutProblem.Id,
                    "Göreve Personel Eklendi",
                    $"'{mevcutProblem.Baslik}' görevine yeni bir ekip arkadaşı dahil edildi.");
            }

            // Senaryo 3: Görevden çıkarılan kişiye bildirim
            foreach (var personelId in cikarilanPersoneller)
            {
                await _bildirimServisi.BildirimGonderAsync(
                    personelId,
                    mevcutProblem.Id,
                    "Görevden Alındınız",
                    $"'{mevcutProblem.Baslik}' başlıklı görevdeki atamanız kaldırıldı.");
            }

            // Senaryo 4: Öncelik değiştiğinde (Görevde güncel bulunan tüm personellere)
            if (eskiOncelik != mevcutProblem.Oncelik && yeniPersonelIdler.Any())
            {
                await _bildirimServisi.TopluBildirimGonderAsync(
                    yeniPersonelIdler,
                    mevcutProblem.Id,
                    "Öncelik Değişti",
                    $"'{mevcutProblem.Baslik}' görevinin önceliği '{mevcutProblem.Oncelik}' olarak güncellendi.");
            }

            // Senaryo 4 & 5: Durum değiştiğinde veya işlem bittiğinde
            if (eskiDurum != mevcutProblem.Durum && yeniPersonelIdler.Any())
            {
                if (mevcutProblem.Durum == ProblemDurumu.Cozuldu)
                {
                    // Senaryo 5: İşlem bittiğinde
                    await _bildirimServisi.TopluBildirimGonderAsync(
                        yeniPersonelIdler,
                        mevcutProblem.Id,
                        "Task Tamamlandı",
                        $"'{mevcutProblem.Baslik}' görevi çözüldü olarak işaretlendi.");
                }
                else
                {
                    // Senaryo 4: Durum güncellendiğinde
                    await _bildirimServisi.TopluBildirimGonderAsync(
                        yeniPersonelIdler,
                        mevcutProblem.Id,
                        "Durum Güncellendi",
                        $"'{mevcutProblem.Baslik}' görevinin durumu '{mevcutProblem.Durum}' olarak güncellendi.");
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task PersonelListesiniYukleAsync(List<int> seciliIds)
        {
            var personeller = await _context.Personeller
                .Select(p => new { p.Id, p.AdSoyad })
                .ToListAsync();

            PersonelListesi = new MultiSelectList(personeller, "Id", "AdSoyad", seciliIds);
        }
    }
}