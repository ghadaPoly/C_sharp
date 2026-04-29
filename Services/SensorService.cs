using DashboardData.Models;
using DashboardData.Data; // Pour accéder à AppDbContext
using Microsoft.EntityFrameworkCore; // Indispensable pour les méthodes Async d'EF Core

namespace DashboardData.Services;

public class SensorService : ISensorService
{
    private readonly AppDbContext _context;

    // L'Injection de dépendance fait le travail ici :
    // Quand Blazor crée le SensorService, il lui passe automatiquement le DbContext.
    public SensorService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<SensorData>> GetSensorsAsync()
{
    // EF Core traduit Include par un JOIN SQL vers la table Location
    return await _context.Sensors
        .Include(s => s.Location) 
        .ToListAsync();
}

    public async Task AddSensorAsync(SensorData sensor)
{
    sensor.LastUpdate = DateTime.Now;
    
    sensor.Values.Add(new SensorValueHistory {
        Value = sensor.Value,
        Timestamp = DateTime.Now
    });

    _context.Sensors.Add(sensor);
    await _context.SaveChangesAsync();
}
public async Task<List<SensorData>> GetCriticalSensorsAsync(double threshold)
{
    return await _context.Sensors
        .Include(s => s.Location)
        .Where(s => s.Value > threshold) 
        .OrderByDescending(s => s.Value) 
        .ToListAsync();                  
}
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Sensors.CountAsync();
    }

    public async Task<double> GetAverageValueAsync()
    {
        if (!await _context.Sensors.AnyAsync())
            return 0;

        return await _context.Sensors.AverageAsync(s => s.Value);
    }

    public async Task<double> GetMaxValueAsync()
    {
        if (!await _context.Sensors.AnyAsync())
            return 0;

        return await _context.Sensors.MaxAsync(s => s.Value);
    }
    public async Task<List<Location>> GetLocationsAsync()
{
    return await _context.Locations.ToListAsync();
}

public async Task<SensorData?> GetSensorByIdAsync(int id)
{
    return await _context.Sensors.FindAsync(id);
}


public async Task UpdateSensorAsync(SensorData sensor)
{
    sensor.LastUpdate = DateTime.Now; 
    
    sensor.Values.Add(new SensorValueHistory {
        Value = sensor.Value,
        Timestamp = DateTime.Now
    });

    _context.Sensors.Update(sensor);
    await _context.SaveChangesAsync();
}

public async Task DeleteSensorAsync(int id)
{
    var sensor = await _context.Sensors.FindAsync(id);
    if (sensor != null)
    {
        _context.Sensors.Remove(sensor);
        await _context.SaveChangesAsync();
    }
}

    public async Task ReloadSensorAsync(SensorData sensor)
    {
        await _context.Entry(sensor).ReloadAsync();
    }
    public async Task<List<LocationStat>> GetAverageValueByLocationAsync()
{
    // EF Core traduit ceci en : SELECT Location, AVG(Value) FROM Sensors GROUP BY Location
    return await _context.Sensors
        .Include(s => s.Location)
        .GroupBy(s => s.Location.Name)
        .Select(g => new LocationStat 
        { 
            LocationName = g.Key ?? "Inconnu", 
            AverageValue = g.Average(s => s.Value) 
        })
        .ToListAsync();
}

public async Task<List<LocationCountStat>> GetSensorCountByLocationAsync()
{
    return await _context.Sensors
        .GroupBy(s => s.Location.Name)
        .Select(g => new LocationCountStat
        {
            LocationName = g.Key,
            Count = g.Count()
        })
        .ToListAsync();
}

public async Task<List<SensorData>> SearchSensorsAsync(string? locationName, string? searchText, bool showCriticalOnly = false)
{
    IQueryable<SensorData> query = _context.Sensors.Include(s => s.Location).AsQueryable();

    if (!string.IsNullOrEmpty(locationName))
        query = query.Where(s => s.Location.Name == locationName);

    if (!string.IsNullOrEmpty(searchText))
        query = query.Where(s => s.Name.Contains(searchText));

    if (showCriticalOnly)
        query = query.Where(s => s.Value > 30.0);

    return await query.ToListAsync();
}
}
