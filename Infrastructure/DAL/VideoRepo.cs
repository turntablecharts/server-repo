using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class VideoRepo : IVideoRepo
    {
        private TtcDbContext _db;
        public VideoRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async Task<VideoItem> AddVideo(VideoItem item)
        {
            await _db.VideoItems.AddAsync(item);
            await _db.SaveChangesAsync();

            return item;
            
        }

        public async void DeleteVideo(int id)
        {
            var video = await _db.VideoItems.FirstOrDefaultAsync(m=> m.Id == id);
            _db.VideoItems.Remove(video);
            await _db.SaveChangesAsync();
        }

        public VideoItem EditVideo(VideoItem item, int id)
        {
            var video = _db.VideoItems.AsNoTracking().FirstOrDefault(m => m.Id == id);
            video = item; 
            video.Id = id;
            _db.VideoItems.Update(video);
            _db.SaveChanges();

            return video;
        }

        public async Task<IList<VideoItem>> GetAllVideos()
        {
            return await _db.VideoItems.ToListAsync();
        }

        public async Task<VideoItem> GetOneVideo(int id)
        {
            return await _db.VideoItems.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}