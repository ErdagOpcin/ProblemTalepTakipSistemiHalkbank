using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;
using ProblemTalepTakipSistemiHalkbank.Services;

namespace ProblemTalepTakipSistemiHalkbank.Pages.Personeller
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailServisi _emailServisi;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailServisi emailServisi)
        {
            _context = context;
            _userManager = userManager;
            _emailServisi = emailServisi;
        }

        [BindProperty]
        public Personel Personel { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Personel.IdentityUserId");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var mevcutUser = await _userManager.FindByEmailAsync(Personel.Email);
            if (mevcutUser != null)
            {
                ModelState.AddModelError("Personel.Email", "Bu e-posta adresi ile kayıtlı bir kullanıcı zaten mevcut.");
                return Page();
            }

            // 1. Rastgele güçlü şifre üret
            var uretilenSifre = RastgeleSifreUret(10);

            // 2. Identity kullanıcısını aç
            var yeniUser = new IdentityUser
            {
                UserName = Personel.Email,
                Email = Personel.Email,
                EmailConfirmed = true
            };

            var sonuc = await _userManager.CreateAsync(yeniUser, uretilenSifre);
            if (!sonuc.Succeeded)
            {
                foreach (var error in sonuc.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // 3. Personeli kaydet
            Personel.IdentityUserId = yeniUser.Id;
            _context.Personeller.Add(Personel);
            await _context.SaveChangesAsync();

            // 4. Şifreyi doğrudan personele mail olarak gönder (Admin görmüyor!)
            var mailIcerigi = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #0d6efd;'>Halkbank Problem & Talep Takip Sistemi</h2>
                    <p>Sayın <strong>{Personel.AdSoyad}</strong>,</p>
                    <p>Sistem üzerinde hesabınız başarıyla tanımlanmıştır. Giriş bilgileriniz aşağıdadır:</p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 6px; margin: 15px 0;'>
                        <p style='margin: 5px 0;'><strong>E-posta:</strong> {Personel.Email}</p>
                        <p style='margin: 5px 0;'><strong>Geçici Şifre:</strong> <span style='font-family: monospace; font-size: 16px; color: #dc3545;'>{uretilenSifre}</span></p>
                    </div>
                    <p style='color: #6c757d; font-size: 12px;'>Güvenliğiniz için ilk girişinizden sonra şifrenizi profil ayarlarından değiştirmeniz önerilir.</p>
                </div>";

            try
            {
                await _emailServisi.SendEmailAsync(Personel.Email, "Halkbank Giriş Bilgileriniz", mailIcerigi);
                TempData["BasariMesaji"] = $"{Personel.AdSoyad} başarıyla kaydedildi. Giriş şifresi e-posta adresine güvenli bir şekilde gönderildi.";
            }
            catch (Exception)
            {
                // SMTP hatası olursa admin haberdar edilsin
                TempData["BasariMesaji"] = $"{Personel.AdSoyad} kaydedildi ancak e-posta gönderilirken bir hata oluştu. Lütfen SMTP ayarlarını kontrol ediniz.";
            }

            return RedirectToPage("./Index");
        }

        private string RastgeleSifreUret(int uzunluk = 10)
        {
            const string kucukHarfler = "abcdefghijklmnopqrstuvwxyz";
            const string buyukHarfler = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string rakamlar = "0123456789";
            const string ozelKarakterler = "!@$?_-*";

            var rnd = new Random();
            var sifreKarakterleri = new List<char>
            {
                buyukHarfler[rnd.Next(buyukHarfler.Length)],
                kucukHarfler[rnd.Next(kucukHarfler.Length)],
                rakamlar[rnd.Next(rakamlar.Length)],
                ozelKarakterler[rnd.Next(ozelKarakterler.Length)]
            };

            string tumKarakterler = buyukHarfler + kucukHarfler + rakamlar + ozelKarakterler;
            for (int i = 4; i < uzunluk; i++)
            {
                sifreKarakterleri.Add(tumKarakterler[rnd.Next(tumKarakterler.Length)]);
            }

            return new string(sifreKarakterleri.OrderBy(_ => rnd.Next()).ToArray());
        }
    }
}