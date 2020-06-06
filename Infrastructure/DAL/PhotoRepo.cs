using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class PhotoRepo : IPhotoRepo
    {
        private TtcDbContext _db;
        public PhotoRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async Task<PhotoItem> AddPhoto(PhotoItem item)
        {
            await _db.PhotoItems.AddAsync(item);
            await _db.SaveChangesAsync();

            return item;
        }

        public async void DeletePhoto(int id)
        {
             var photo = await _db.PhotoItems.FirstOrDefaultAsync(m => m.Id == id);
            _db.PhotoItems.Remove(photo);
            await _db.SaveChangesAsync();
        }

        public PhotoItem EditPhoto(PhotoItem item, int id)
        {
            var photo = _db.PhotoItems.AsNoTracking().FirstOrDefault(m => m.Id == id);
            photo = item;
            photo.Id = id; 

            _db.PhotoItems.Update(photo);
            _db.SaveChanges();

            return item;

        }

        public async Task<List<PhotoItem>> GetAllPhotos()
        {
             return await _db.PhotoItems.ToListAsync();
        }

        public async Task<PhotoItem> GetOnePhoto(int id)
        {
            return await _db.PhotoItems.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}