using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public interface IBildirimServisi
    {
        Task BildirimGonderAsync(int personelId, int problemId, string baslik, string mesaj);
        Task TopluBildirimGonderAsync(IEnumerable<int> personelIdler, int problemId, string baslik, string mesaj);
    }
}