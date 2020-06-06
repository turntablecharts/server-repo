using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IPhotoRepo
    {
        Task<PhotoItem> AddPhoto(PhotoItem item);
        void DeletePhoto(int id);
        Task<PhotoItem> GetOnePhoto(int id);
        Task<List<PhotoItem>> GetAllPhotos();
        PhotoItem EditPhoto(PhotoItem item, int id);
    }
}