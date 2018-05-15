using DanfossTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DanfossTest.DAL
{
    public class HomeEntityRepository : Repository<HomeEntity>
    {
        public HomeEntityRepository(DbContextOptions<DataBaseContext> options) : base(options)
        { }

        public override void LoadRelatedEntiti(HomeEntity entity)
        {
            DataBaseContext.Entry(entity).Reference(s => s.WaterMeter).Load();
        }

        public override HomeEntity GetItem(Expression<Func<HomeEntity, bool>> predicate) => base.GetItem(predicate);

        public override async Task<HomeEntity> GetMaxItemAsync()
        {
            var maxMeter = await DataBaseContext.Set<HomeEntity>().Include(s => s.WaterMeter).
                Where(a => a.WaterMeter != null).Select(s => s.WaterMeter.Meter).DefaultIfEmpty(0).MaxAsync();
            return await DataBaseContext.Set<HomeEntity>().Include(s => s.WaterMeter).FirstOrDefaultAsync(s => s.WaterMeter.Meter == maxMeter);
        }

        public override async Task<HomeEntity> GetMinItemAsync()
        {
            var minMeter = await DataBaseContext.Set<HomeEntity>().Include(s => s.WaterMeter).
                Where(a => a.WaterMeter != null).Select(s => s.WaterMeter.Meter).DefaultIfEmpty(0).MinAsync();
            return await DataBaseContext.Set<HomeEntity>().Include(s => s.WaterMeter).FirstOrDefaultAsync(s => s.WaterMeter.Meter == minMeter);
        }
    }
}
