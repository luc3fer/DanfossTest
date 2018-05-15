using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DanfossTest.Models
{
    public class WaterMeter: BaseModelMapping
    {
        public int HomeEntityId { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        [Required]
        public int Meter { get; set; }

        public HomeEntity HomeEntity { get; set; }
    }
}
