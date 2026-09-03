namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public class Personel
    {
        public int Id{ get; set; }
        public string AdSoyad { get; set;} = string.Empty;
        public string Departman { get; set;} = string.Empty;
        public List<Problem> Problemler { get; set; }= new List<Problem>();
    }
}