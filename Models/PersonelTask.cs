namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class PersonelTask
    {
        public int Id { get; set; }

        public string Baslik { get; set; } = string.Empty;

        public string Aciklama { get; set; } = string.Empty;

        // Admin tarafından belirlenen tahmini süre
        // Örn: 3 saat, 2.5 saat
        public decimal PlanlananEfor { get; set; }

        // Personel işi bitirirken/güncellerken girer
        public decimal? HarcananEfor { get; set; }

        public TaskDurumu Durum { get; set; }
            = TaskDurumu.Bekliyor;

        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.Now;

        public DateTime? TamamlanmaTarihi { get; set; }

        // Task hangi personele atanmış?
        public int PersonelId { get; set; }

        public Personel? Personel { get; set; }
    }

    public enum TaskDurumu
    {
        Bekliyor = 0,
        DevamEdiyor = 1,
        Tamamlandi = 2
    }
}