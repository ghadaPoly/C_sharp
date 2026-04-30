using Microsoft.EntityFrameworkCore;
using  DashboardData.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace DashboardData.Data;
//{ } [ ] < > | 
public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { } 
    public DbSet<SensorData> Sensors {get; set; } 
    public DbSet<Location> Locations {get; set; } 
    public DbSet<Tag> Tags {get; set; } 
    public DbSet<SensorValueHistory> SensorValueHistories { get; set; }
}


