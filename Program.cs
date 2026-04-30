using DashboardData.Components;
using DashboardData.Services;
using DashboardData.Models;
using Microsoft.EntityFrameworkCore;
using DashboardData.Data;
using Radzen;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddRadzenComponents();
builder.Services.AddTransient<UserCounterService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    if (await userManager.FindByEmailAsync("admin@data.com") == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = "admin@data.com",
            Email = "admin@data.com",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    if (await userManager.FindByEmailAsync("user@data.com") == null)
    {
        var normalUser = new IdentityUser
        {
            UserName = "user@data.com",
            Email = "user@data.com",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(normalUser, "User123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(normalUser, "User");
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    if (!context.Sensors.Any())
    {
        Console.WriteLine("Generation de données de test");

        var labo = new Location { Name = "Labo", Building = "Bat. A" };
        var usine = new Location { Name = "Usine", Building = "Bat. B" };
        context.Locations.AddRange(labo, usine);

        var tagCritique = new Tag { Label = "Critique" };
        var tagMaintenance = new Tag { Label = "Maintenance" };
        context.Tags.AddRange(tagCritique, tagMaintenance);
        context.SaveChanges();

        var sonde1 = new SensorData
        {
            Name = "Sonde_Alpha",
            Value = 25.4,
            LocationId = labo.Id,
            Tags = new List<Tag> { tagCritique }
        };

        var sonde2 = new SensorData
        {
            Name = "Sonde_Beta",
            Value = 40.2,
            LocationId = usine.Id,
            Tags = new List<Tag> { tagCritique, tagMaintenance }
        };

        context.Sensors.AddRange(sonde1, sonde2);
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm] string email,
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    if (result.Succeeded) return Results.Redirect("/dashboard");
    return Results.Redirect("/login?error=Invalid+credentials");
}).DisableAntiforgery();

// recoit les infos dapres le form
app.MapPost("/api/auth/register", async (
    [FromServices] UserManager<IdentityUser> userManager,
    [FromServices] SignInManager<IdentityUser> signInManager,
    [FromForm] string email,
    [FromForm] string password,
    [FromForm] string confirmPassword) =>
{
    if (password != confirmPassword)
        return Results.Redirect("/inscription?error=Les+mots+de+passe+ne+correspondent+pas");

    var existingUser = await userManager.FindByEmailAsync(email);
    if (existingUser != null)
        return Results.Redirect("/inscription?error=Cet+email+existe+déjà");

    var newUser = new IdentityUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };

    var result = await userManager.CreateAsync(newUser, password);

    if (result.Succeeded)
    {
        // taatih l role User
        await userManager.AddToRoleAsync(newUser, "User");

        // tconnecti directement baaed inscription
        await signInManager.SignInAsync(newUser, isPersistent: false);

        return Results.Redirect("/dashboard");
    }

    // Retourner les erreurs  mot de passe trop faible)
    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
    return Results.Redirect($"/inscription?error={Uri.EscapeDataString(errors)}");

}).DisableAntiforgery();

// deconnexion
app.MapPost("/api/auth/logout", async ([FromServices] SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

app.Run();