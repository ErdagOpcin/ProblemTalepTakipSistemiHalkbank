namespace ProblemTalepTakipSistemiHalkbank.Models
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
}
