/*namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Baslik{ get; set; } = string.Empty;
        public string Detay{ set; get; } = string.Empty;
        public string KimdenGeldi{ get; set; } = string.Empty;
        public string Birim{ get; set; } = string.Empty;
        public string İl{ get; set; } = string.Empty;
        public string Durum{ get; set; } = "Bekliyor";
        public DateTime OlusturulmaTarihi{ get; set; } =DateTime.Now;
        public int? PersonelId{ get; set; }
        public Personel? Personel { get; set; }
    }
}*/
 using System.ComponentModel.DataAnnotations;
 
 namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class Problem
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Baslik { get; set; } = string.Empty;
        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        public string Aciklama { get; set; } = string.Empty;
        public string KimdenGeldi { get; set; } = string.Empty;
        public string Birim { get; set; } = string.Empty;
        [Required(ErrorMessage = "Lütfen bir il seçiniz.")]
        public string Sehir { get; set; } = string.Empty;
        // enum alanı
        public ProblemOncelik Oncelik { get; set; } = ProblemOncelik.Dusuk;
        public ProblemDurumu Durum { get; set; } = ProblemDurumu.Acik;

        // tarih bilgisi
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        // atanacak personel
        [Required(ErrorMessage = "Lütfen problemi atayacağınız personeli seçiniz.")]
        public int? PersonelId { get; set; }
        //Navigation Property: C# tarafında personelin tüm detaylarına erişim sağlar
        public Personel? Personel { get; set; }
    }
}