using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Consumer.Models
{
    public class SensorData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public decimal Temperature { get; set; }
        [Required]
        public decimal Humidity { get; set; }
        [Required]
        public DateTime MeasuredAt { get; set; }
    }
}
