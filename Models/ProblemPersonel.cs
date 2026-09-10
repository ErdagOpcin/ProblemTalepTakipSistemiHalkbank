namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class ProblemPersonel
    {
        public int ProblemId { get; set; }
        public Problem Problem { get; set; } = default!;

        public int PersonelId { get; set; }
        public Personel Personel { get; set; } = default!;
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    }
}