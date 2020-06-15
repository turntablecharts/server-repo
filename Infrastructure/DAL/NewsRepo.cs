using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class NewsRepo : INewsRepo
    {
        private TtcDbContext _db;
        public NewsRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async Task<NewsItem> AddNews(NewsItem news)
        {
            await _db.NewsItems.AddAsync(news);
            await _db.SaveChangesAsync();

            return news;
        }

        public async void DeleteNews(int id)
        {
            var news = await _db.NewsItems.FirstOrDefaultAsync(m => m.Id == id);
            _db.NewsItems.Remove(news);
            await _db.SaveChangesAsync();
        }

        public NewsItem EditNews(NewsItem news, int id)
        {
            var item = _db.NewsItems.AsNoTracking().FirstOrDefault(m => m.Id == id);
            if (item == null) { return null; }
            item = news;
            item.Id = id; 

            _db.NewsItems.Update(item);
            _db.SaveChanges();

            return item;

        }

        public async Task<IList<NewsItem>> GetAllNews()
        {
            return await _db.NewsItems.ToListAsync();
        }

        public async Task<NewsItem> GetOne(int id)
        {
           return await _db.NewsItems.Include(m => m.ttcUser).FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}