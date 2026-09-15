using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public DurumGuncelleModel(ApplicationDbContext context, IBildirimServisi bildirimServisi)
        {
            _context = context;
            _bildirimServisi = bildirimServisi;
        }

        [BindProperty]
        public Problem Problem { get; set; } = default!;

        // Formdan seçilen personel ID'lerini yakalayan liste:
        [BindProperty]
        public List<int> SecilenPersonelIdleri { get; set; } = new();

        // Checkbox listesinde aktif görev sayılarını gösteren DTO listesi:
        public List<PersonelSecimDto> PersonelListesi { get; set; } = new();

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

            // Personelleri ve aktif görev sayılarını yükle
            await PersonelListesiniYukleAsync(problem.Id);

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

            var yeniPersonelIdler = SecilenPersonelIdleri ?? new List<int>();

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

            // Senaryo 5: Durum değiştiğinde veya işlem bittiğinde
            if (eskiDurum != mevcutProblem.Durum && yeniPersonelIdler.Any())
            {
                if (mevcutProblem.Durum == ProblemDurumu.Cozuldu)
                {
                    await _bildirimServisi.TopluBildirimGonderAsync(
                        yeniPersonelIdler,
                        mevcutProblem.Id,
                        "Task Tamamlandı",
                        $"'{mevcutProblem.Baslik}' görevi çözüldü olarak işaretlendi.");
                }
                else
                {
                    await _bildirimServisi.TopluBildirimGonderAsync(
                        yeniPersonelIdler,
                        mevcutProblem.Id,
                        "Durum Güncellendi",
                        $"'{mevcutProblem.Baslik}' görevinin durumu '{mevcutProblem.Durum}' olarak güncellendi.");
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task PersonelListesiniYukleAsync(int problemId)
        {
            PersonelListesi = await _context.Personeller
                .Select(p => new PersonelSecimDto
                {
                    Id = p.Id,
                    AdSoyad = p.AdSoyad,
                    Departman = p.Departman,
                    // Çözülmemiş işleri say:
                    AktifGorevSayisi = p.ProblemPersoneller.Count(pp =>
                        pp.Problem.Durum != ProblemDurumu.Cozuldu),
                    SeciliMi = p.ProblemPersoneller.Any(pp => pp.ProblemId == problemId)
                })
                .OrderBy(p => p.AktifGorevSayisi) // En az işi olandan en çoka
                .ToListAsync();

            SecilenPersonelIdleri = PersonelListesi
                .Where(p => p.SeciliMi)
                .Select(p => p.Id)
                .ToList();
        }
    }

    public class PersonelSecimDto
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Departman { get; set; } = string.Empty;
        public int AktifGorevSayisi { get; set; }
        public bool SeciliMi { get; set; }
    }
}