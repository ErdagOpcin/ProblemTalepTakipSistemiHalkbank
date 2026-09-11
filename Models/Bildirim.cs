using System;

namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class Bildirim
    {
        public int Id { get; set; }
        // Bildirimin kime gittiği
        public int PersonelId { get; set; }
        public Personel? Personel { get; set; }
        // İlgili problem (Tıklandığında yönlendirmek için)
        public int ProblemId { get; set; }
        public Problem? Problem { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Mesaj { get; set; } = string.Empty;
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        public bool OkunduMu { get; set; } = false;
    }
}