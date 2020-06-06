using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IMediaRepo
    {
        Task<MediaItem> Add(MediaItem item);
        Task<MediaItem> GetOne(int id);
        void DeleteMedia(int id);
        Task<IList<MediaItem>> GetAllMedia();
    }
}