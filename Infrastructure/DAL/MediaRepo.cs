using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL {
    public class MediaRepo : IMediaRepo {
        private TtcDbContext _db;
        private IBlobRepo _blob;
        public MediaRepo (TtcDbContext db, IBlobRepo blob) {
            _db = db;
            _blob = blob;
        }
        public async Task<MediaItem> Add (MediaItem item, IFormFile file, string blobKey) 
        {
            string imageUri = GetFileUploadBlobReturnsLink (file, blobKey);
            item.ImageLink = imageUri;
            
            await _db.MediaItems.AddAsync (item);
            await _db.SaveChangesAsync ();

            return item;
        }

        public async void DeleteMedia (int id) {
            var media = await _db.MediaItems.FirstOrDefaultAsync (m => m.Id == id);
            _db.MediaItems.Remove (media);
            await _db.SaveChangesAsync ();
        }

        public async Task<IList<MediaItem>> GetAllMedia () {
            return await _db.MediaItems.ToListAsync ();
        }

        public async Task<MediaItem> GetOne (int id) {
            return await _db.MediaItems.FirstOrDefaultAsync (m => m.Id == id);
        }

        public string GetFileUploadBlobReturnsLink (IFormFile file, string blobKey) {
            var fileName = file.FileName;
            string mime = file.ContentType;
            byte[] data = GetBytes (file);

            return _blob.UploadFileToBlob (fileName, data, mime, blobKey);
        }

        public static byte[] GetBytes (IFormFile file) {
            using (var ms = new MemoryStream ()) {
                file.CopyTo (ms);
                return ms.ToArray ();
            }
        }
    }
}