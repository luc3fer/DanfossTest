using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DanfossTest.Models
{
    public class HomeEntity : BaseModelMapping
    {
        [Required]
        public string Address { get; set; }

        public WaterMeter WaterMeter { get; set; }
    }
}
