using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Services;

var builder = WebApplication.CreateBuilder(args);


// Veritabanı bağlantısı
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection bulunamadı."
    );
}


// Entity Framework ve SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(connectionString)
);


// Identity ayarları
builder.Services.AddDefaultIdentity<IdentityUser>(
    options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// HttpClient servisleri
builder.Services.AddHttpClient<PasswordLeakService>();
builder.Services.AddHttpClient<CityApiService>();


// Giriş yapmamış kullanıcıların
// uygulama sayfalarına erişmesini engelle
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});


builder.Services.AddRazorPages();


var app = builder.Build();


// Uygulamada kullanılacak rollerin
// veritabanında bulunmasını garanti eder
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();


    string[] roles =
    {
        "Admin",
        "Personel"
    };


    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role)
            );
        }
    }
}


// HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapStaticAssets()
    .AllowAnonymous();


app.MapRazorPages()
    .WithStaticAssets();


app.Run();