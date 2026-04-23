using Microsoft.EntityFrameworkCore;
using  DashboardData.Models;
namespace DashboardData.Data;
//{ } [ ] < > | 
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { } 
    public DbSet<SensorData> Sensors {get; set; } 
    public DbSet<Location> Locations {get; set; } 
    public DbSet<Tag> Tags {get; set; } 
    public DbSet<SensorValueHistory> SensorValueHistories { get; set; }
}
