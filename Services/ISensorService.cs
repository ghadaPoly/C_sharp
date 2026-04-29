using DashboardData.Models;
namespace DashboardData.Services;
public interface ISensorService
    {
    Task<List<SensorData>> GetSensorsAsync();
    Task AddSensorAsync(SensorData sensor);
    Task<List<SensorData>> GetCriticalSensorsAsync(double threshold);

    Task<int> GetTotalCountAsync();
    Task<double> GetAverageValueAsync();
    Task<double> GetMaxValueAsync();
    Task<List<Location>> GetLocationsAsync();
    Task<SensorData?> GetSensorByIdAsync(int id);
    Task ReloadSensorAsync(SensorData sensor);
    Task UpdateSensorAsync(SensorData sensor);
    Task<List<LocationCountStat>> GetSensorCountByLocationAsync();
    Task DeleteSensorAsync(int id);
    Task<List<LocationStat>> GetAverageValueByLocationAsync();
    Task<List<SensorData>> SearchSensorsAsync(string? locationFilter, string? searchText, bool criticalOnly = false);
    }
