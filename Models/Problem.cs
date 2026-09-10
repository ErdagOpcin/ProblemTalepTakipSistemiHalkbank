using System.ComponentModel.DataAnnotations;

namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class Problem
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(
            100,
            ErrorMessage = "Başlık en fazla 100 karakter olabilir."
        )]
        public string Baslik { get; set; } = string.Empty;


        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        public string Aciklama { get; set; } = string.Empty;


        public string KimdenGeldi { get; set; } = string.Empty;


        public string Birim { get; set; } = string.Empty;


        [Required(ErrorMessage = "Lütfen bir il seçiniz.")]
        public string Sehir { get; set; } = string.Empty;


        [Required(ErrorMessage = "Lütfen bir ilçe seçiniz.")]
        public string Ilce { get; set; } = string.Empty;


        public ProblemOncelik Oncelik { get; set; }
            = ProblemOncelik.Dusuk;


        public ProblemDurumu Durum { get; set; }
            = ProblemDurumu.Bekliyor;


        // Problem sisteme eklendiği anda otomatik oluşur.
        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.Now;


        // Problem "Çözüldü" durumuna geçtiği anda
        // sistem tarafından otomatik doldurulur.
        public DateTime? CozulmeTarihi { get; set; }


        [Required(
            ErrorMessage =
                "Lütfen problemi atayacağınız personeli seçiniz."
        )]
        public ICollection<ProblemPersonel> ProblemPersoneller { get; set; } = new List<ProblemPersonel>();
    }
}