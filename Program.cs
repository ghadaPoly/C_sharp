using DashboardData.Components;
using DashboardData.Services;
using DashboardData.Models;
using Microsoft.EntityFrameworkCore;
using DashboardData.Data;
using Radzen ;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ISensorService,SensorService>();
builder.Services.AddRadzenComponents();
builder.Services.AddTransient<UserCounterService>()
;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    if (!context.Sensors.Any())
    {
        Console.WriteLine("Generation de données de test ");

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



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
