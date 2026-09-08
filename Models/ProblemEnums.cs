using System.ComponentModel.DataAnnotations;

namespace ProblemTalepTakipSistemiHalkbank.Models
{
    public enum ProblemDurumu
    {
        [Display(Name = "Bekliyor")]
        Bekliyor = 1,

        [Display(Name = "İşleme Alındı")]
        IslemeAlindi = 2,

        [Display(Name = "Çözüldü")]
        Cozuldu = 3
    }

    public enum ProblemOncelik
    {
        [Display(Name = "Düşük")]
        Dusuk = 1,

        [Display(Name = "Orta")]
        Orta = 2,

        [Display(Name = "Yüksek")]
        Yuksek = 3,

        [Display(Name = "Kritik")]
        Kritik = 4
    }
}