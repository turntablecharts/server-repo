using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class ChartRepo : IChartRepo
    {
        private TtcDbContext _db;
        public ChartRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async Task<Chart> AddChart(Chart chart)
        {
            await _db.Charts.AddAsync(chart);
            await _db.SaveChangesAsync();

            return chart;
        }

        public async void DeleteChart(int id)
        {
             var chart = await _db.Charts.FirstOrDefaultAsync(m=> m.Id == id);
            _db.Charts.Remove(chart);
            await _db.SaveChangesAsync();
        }

        public async Task<IList<Chart>> GetAllCharts()
        {
            return await _db.Charts.ToListAsync();
        }

        public async Task<Chart> GetOne(int id)
        {
            return await _db.Charts.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}