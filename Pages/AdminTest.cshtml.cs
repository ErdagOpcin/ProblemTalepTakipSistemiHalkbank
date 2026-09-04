using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProblemTalepTakipSistemiHalkbank.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminTestModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}