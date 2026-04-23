using System.ComponentModel.DataAnnotations;
namespace DashboardData.Models;
//{ } [ ] < > | 
public class Tag
{
    [Key]
    public int id {get; set; } 
    [Required] [StringLength(30)]
    public string Label {get; set; } 
    public string? Building {get; set; } 
    public ICollection<SensorData> Sensors {get; set; } = new List<SensorData>();

}
