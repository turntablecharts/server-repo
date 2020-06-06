using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class LogRepo : ILogRepo
    {
        private TtcDbContext _db;
        public LogRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async void AddToLog(Log logItem)
        {
            await _db.Logs.AddAsync(logItem);
            await _db.SaveChangesAsync();
        }

        public async Task<IList<Log>> GetLogs()
        {
            return await _db.Logs.ToListAsync();
        }
    }
}