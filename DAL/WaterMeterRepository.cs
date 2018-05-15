using DanfossTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DanfossTest.DAL
{
    public class WaterMeterRepository: Repository<WaterMeter>
    {
        public WaterMeterRepository(DbContextOptions<DataBaseContext> options) : base(options)
        { }

        public override WaterMeter GetItem(Expression<Func<WaterMeter, bool>> predicate) => base.GetItem(predicate);

        public override async Task<WaterMeter> GetMaxItemAsync()
        {
            var maxMeter = await DataBaseContext.Set<WaterMeter>().MaxAsync(s => s.Meter);
            return await DataBaseContext.Set<WaterMeter>().FirstAsync(s => s.Meter == maxMeter);
        }

        public override async Task<WaterMeter> GetMinItemAsync()
        {
            var minMeter = await DataBaseContext.Set<WaterMeter>().MinAsync(s => s.Meter);
            return await DataBaseContext.Set<WaterMeter>().FirstAsync(s => s.Meter == minMeter);
        }

        public override void LoadRelatedEntiti(WaterMeter waterMeter)
        {
            DataBaseContext.Entry(waterMeter).Reference(s => s.HomeEntity).Load();
        }
    }
}
