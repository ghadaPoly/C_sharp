
using System. ComponentModel.DataAnnotations;
namespace DashboardData.Models;
using System.ComponentModel.DataAnnotations;

public class SensorData
{

[Key] 
public int Id { get; set; }

[Required] [StringLength(50, MinimumLength = 3, ErrorMessage="Le nom doit faire entre 3 et 50 caracteres ")]
public string Name { get; set; }


public string Type { get; set; } = "Temperature";
[Range(-50.0, 150.0)]
public double Value { get; set; }

public DateTime LastUpdate { get; set; } = DateTime.Now;
[Range(1, int.MaxValue,ErrorMessage ="veuillez selectionner un lieu valide")] 
public int LocationId { get; set; }
public Location Location { get; set; }

public ICollection<Tag> Tags { get; set; } = new List<Tag>();
public ICollection<SensorValueHistory> Values { get; set; } = new List<SensorValueHistory>();


}
public class LocationStat
{
    public string LocationName { get; set; }
    public double AverageValue { get; set; }
}

