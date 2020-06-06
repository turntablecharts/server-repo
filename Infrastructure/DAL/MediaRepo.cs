using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class MediaRepo : IMediaRepo
    {
        private TtcDbContext _db;
        public MediaRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async Task<MediaItem> Add(MediaItem item)
        {
            await _db.MediaItems.AddAsync(item);
            await _db.SaveChangesAsync();

            return item;
        }

        public async void DeleteMedia(int id)
        {
            var media = await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == id);
            _db.MediaItems.Remove(media);
            await _db.SaveChangesAsync();
        }

        public async Task<IList<MediaItem>> GetAllMedia()
        {
            return await _db.MediaItems.ToListAsync();
        }

        public async Task<MediaItem> GetOne(int id)
        {
            return await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}