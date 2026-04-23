using System;

namespace DashboardData.Models
{
    public class SensorValueHistory
    {
        public int Id { get; set; } 

        public double Value { get; set; }

        public DateTime Timestamp { get; set; }

        public int SensorDataId { get; set; }

        public SensorData SensorData { get; set; } = null!;
    }
}
