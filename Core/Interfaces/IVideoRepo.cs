using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IVideoRepo
    {
        Task<VideoItem> AddVideo(VideoItem item);
        Task<VideoItem> GetOneVideo(int id);
        Task<IList<VideoItem>> GetAllVideos();
        VideoItem EditVideo(VideoItem item, int id);
        void DeleteVideo(int id);

    }
}