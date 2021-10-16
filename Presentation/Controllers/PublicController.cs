using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/public")]
    public class PublicController : ControllerBase
    {
        private readonly IGenericRepository<NewsCategory> _newsRepo;
        private readonly IGenericRepository<Chart> _chartRepo;
        private readonly IGenericRepository<PhotoCategory> _photoRepo;

        public PublicController( IGenericRepository<NewsCategory> newsRepo,
          IGenericRepository<Chart> chartRepo,
          IGenericRepository<PhotoCategory> photoRepo)
        {
            _newsRepo = newsRepo;
            _chartRepo = chartRepo;
            _photoRepo = photoRepo;
        }

        [HttpGet("homepage")]
        public IActionResult GetHomePage([FromQuery]string photoCategory, [FromQuery]string newsCategory, [FromQuery]string chartCategory)
        {
            //load photo
            photoCategory = photoCategory != null ? photoCategory : "Turntable NXT";
            newsCategory = newsCategory != null ? newsCategory : "Chart News";
            chartCategory = chartCategory != null ? chartCategory : "Turntable Top 50";

            int pageNumber = 1;
            int pageSize = 10;
            int skipSize = ((int)pageNumber - 1) * pageSize;

            var photos = _photoRepo.GetWithInclude(m => m.Name == photoCategory, "Photos")
                .FirstOrDefault()
                .Photos
                .OrderByDescending(m => m.DateCreated)
                .Skip(skipSize).Take(pageSize).ToList();

            //load news
            var news = _newsRepo.GetWithInclude(m => m.Name == newsCategory, "NewsList")
                .FirstOrDefault()
                .NewsList
                .OrderByDescending(m => m.DateCreated)
                .Skip(skipSize).Take(pageSize).ToList();

            //load charts
            var latest = _chartRepo.GetAll().OrderByDescending(m => m.DateCreated).Where(m => m.Category.Contains(chartCategory ?? "Turntable Top 50")).FirstOrDefault();

            var result = _chartRepo.GetWithInclude(m => m.Id == latest.Id, "ChartItems").FirstOrDefault();

            var chartToFrontend = new Chart
            {
                Id = result.Id,
                DateCreated = result.DateCreated,
                Week = result.Week,
                ChartItems = result.ChartItems.OrderBy(m => m.Rank).Take(5).ToList(),
                Category = result.Category,
                Genre = result.Genre,
                HeaderVideoUrl = result.HeaderVideoUrl
            };

            var response = new HomepageDto
            {
                news = news, 
                photos = photos, 
                chart = chartToFrontend
            };

            return Ok(response);

        }
    }
}