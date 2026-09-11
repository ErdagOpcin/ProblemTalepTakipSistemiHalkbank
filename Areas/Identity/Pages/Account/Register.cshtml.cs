#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Models;
using ProblemTalepTakipSistemiHalkbank.Services;

namespace ProblemTalepTakipSistemiHalkbank.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUserStore<IdentityUser> _userStore;

        private readonly IUserEmailStore<IdentityUser> _emailStore;

        private readonly ILogger<RegisterModel> _logger;

        private readonly PasswordLeakService _passwordLeakService;

        private readonly ApplicationDbContext _context;


        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            PasswordLeakService passwordLeakService,
            ApplicationDbContext context)
        {
            _userManager = userManager;

            _userStore = userStore;

            _emailStore = GetEmailStore();

            _signInManager = signInManager;

            _logger = logger;

            _passwordLeakService = passwordLeakService;

            _context = context;
        }


        [BindProperty]
        public InputModel Input { get; set; }


        public string ReturnUrl { get; set; }


        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public class InputModel
        {
            [Required]
            [Display(Name = "Ad Soyad")]
            public string AdSoyad { get; set; }


            [Required]
            [Display(Name = "Departman")]
            public string Departman { get; set; }


            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [Required]
            [StringLength(
                100,
                ErrorMessage =
                    "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }


            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare(
                "Password",
                ErrorMessage =
                    "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(
            string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins =
                (
                    await _signInManager
                        .GetExternalAuthenticationSchemesAsync()
                )
                .ToList();
        }


        public async Task<IActionResult> OnPostAsync(
            string returnUrl = null)
        {
            returnUrl ??=
                Url.Content("~/");


            ExternalLogins =
                (
                    await _signInManager
                        .GetExternalAuthenticationSchemesAsync()
                )
                .ToList();


            if (!ModelState.IsValid)
            {
                return Page();
            }


            // =================================================
            // ŞİFRE SIZINTI KONTROLÜ
            // =================================================

            if (
                await _passwordLeakService
                    .IsPasswordLeakedAsync(Input.Password)
            )
            {
                ModelState.AddModelError(
                    "Input.Password",
                    "Bu şifre daha önce veri sızıntılarında görülmüş. " +
                    "Lütfen başka bir şifre seçin."
                );

                return Page();
            }


            // =================================================
            // IDENTITY USER OLUŞTUR
            // =================================================

            var user = CreateUser();


            await _userStore.SetUserNameAsync(
                user,
                Input.Email,
                CancellationToken.None
            );


            await _emailStore.SetEmailAsync(
                user,
                Input.Email,
                CancellationToken.None
            );


            var result =
                await _userManager.CreateAsync(
                    user,
                    Input.Password
                );


            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "User created a new account with password."
                );


                // =============================================
                // PERSONEL ROLÜ VER
                // =============================================

                await _userManager.AddToRoleAsync(
                    user,
                    "Personel"
                );


                // =============================================
                // PERSONEL TABLOSUNA KAYIT OLUŞTUR
                // =============================================

                var personel =
                    new Personel
                    {
                        AdSoyad =
                            Input.AdSoyad,

                        Departman =
                            Input.Departman,

                        IdentityUserId =
                            user.Id
                    };


                _context.Personeller.Add(
                    personel
                );


                await _context.SaveChangesAsync();


                // =============================================
                // GİRİŞ YAPTIR
                // =============================================

                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false
                );


                return LocalRedirect(
                    returnUrl
                );
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description
                );
            }


            return Page();
        }


        private IdentityUser CreateUser()
        {
            try
            {
                return Activator
                    .CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of " +
                    $"'{nameof(IdentityUser)}'."
                );
            }
        }


        private IUserEmailStore<IdentityUser>
            GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "The default UI requires a user store " +
                    "with email support."
                );
            }


            return
                (IUserEmailStore<IdentityUser>)
                _userStore;
        }
    }
}