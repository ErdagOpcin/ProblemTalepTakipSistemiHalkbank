using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProblemTalepTakipSistemiHalkbank.Data;
using ProblemTalepTakipSistemiHalkbank.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection bulunamadı.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


// HttpClient servisleri
builder.Services.AddHttpClient<PasswordLeakService>();
builder.Services.AddHttpClient<CityApiService>();


// Giriş yapmamış kullanıcıların sayfalara erişmesini engelle
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRazorPages();

var app = builder.Build();


// Roller ve test kullanıcıları için başlangıç ayarları
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var dbContext =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


    // Admin ve Personel rollerini oluştur
    string[] roles = { "Admin", "Personel" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role)
            );
        }
    }


    // test1@example.com kullanıcısını Admin yap
    var adminUser =
        await userManager.FindByEmailAsync("test1@example.com");

    if (adminUser != null &&
        !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(
            adminUser,
            "Admin"
        );
    }


    // test3@example.com kullanıcısını Personel yap
    var personelUser =
        await userManager.FindByEmailAsync("test3@example.com");

    if (personelUser != null)
    {
        if (!await userManager.IsInRoleAsync(
                personelUser,
                "Personel"))
        {
            await userManager.AddToRoleAsync(
                personelUser,
                "Personel"
            );
        }


        // test3 kullanıcısını Ahmet Yılmaz personeli ile eşleştir
        var personel = await dbContext.Personeller
            .FirstOrDefaultAsync(
                p => p.AdSoyad == "Ahmet Yılmaz"
            );

        if (personel != null &&
            string.IsNullOrEmpty(personel.IdentityUserId))
        {
            personel.IdentityUserId = personelUser.Id;

            await dbContext.SaveChangesAsync();
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