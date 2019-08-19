using System;
using System.Collections.Generic;
using System.Text;

namespace RoomTempMQTTConsumer.Entities
{
    class MetricData
    {
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public DateTime MeasuredAt { get; set; }
    }
}
