using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface INewsRepo
    {
        Task<NewsItem> AddNews(NewsItem news);
        Task<NewsItem> GetOne(int id);
        Task<IList<NewsItem>> GetAllNews();
        NewsItem EditNews(NewsItem news, int id);
        void DeleteNews(int id);

    }
}