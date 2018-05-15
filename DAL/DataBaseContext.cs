using DanfossTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DanfossTest.DAL
{
    public class DataBaseContext : DbContext
    {
        public DbSet<HomeEntity> HomeEntitys { get; set; }
        public DbSet<WaterMeter> WaterMeters { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        { }
    }
}
