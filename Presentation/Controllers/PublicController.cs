using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Core.Interfaces;
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
        private readonly IGenericRepository<PowerListNomination> _powerListNomination;
        private readonly ICacheService _cacheService;

        public PublicController(IGenericRepository<NewsCategory> newsRepo,
          IGenericRepository<Chart> chartRepo,
          IGenericRepository<PhotoCategory> photoRepo,
          IGenericRepository<PowerListNomination> powerListNomination,
          ICacheService cacheService)
        {
            _newsRepo = newsRepo;
            _chartRepo = chartRepo;
            _photoRepo = photoRepo;
            _powerListNomination = powerListNomination;
            _cacheService = cacheService;
        }

        [HttpPost("powerList")]
        public async Task<IActionResult> AddToPowerList([FromBody] PowerListNomination power)
        {
            power.Id = System.Guid.NewGuid();
            power.DateAdded = System.DateTime.Now;
            var result = await _powerListNomination.AddAsync(power);
            return Ok(new { StatusCode = 200, Message = "Data Successfully saved" });
        }

        [HttpGet("homepage")]
        public async Task<IActionResult> GetHomePage([FromQuery] string photoCategory, [FromQuery] string newsCategory, [FromQuery] string chartCategory)
        {
            photoCategory = photoCategory ?? "Turntable NXT";
            newsCategory = newsCategory ?? "Chart News";
            chartCategory = chartCategory ?? "Turntable Top 50";

            string cacheKey = $"homepage_{photoCategory}_{newsCategory}_{chartCategory}".ToLower();

            var response = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    int pageNumber = 1;
                    int pageSize = 10;
                    int skipSize = ((int)pageNumber - 1) * pageSize;

                    var photosResult = _photoRepo.GetWithInclude(m => m.Name == photoCategory, "Photos").FirstOrDefault();
                    var photos = photosResult?.Photos.Where(m => m.IsDeleted == false)
                        .OrderByDescending(m => m.DateCreated)
                        .Skip(skipSize).Take(pageSize).ToList() ?? new List<Photo>();

                    var newsResult = _newsRepo.GetWithInclude(m => m.Name == newsCategory, "NewsList").FirstOrDefault();
                    var news = newsResult?.NewsList.Where(m => m.IsDeleted == false)
                        .OrderByDescending(m => m.DateCreated)
                        .Skip(skipSize).Take(pageSize).ToList() ?? new List<News>();

                    var latest = await _chartRepo.GetAsync(m => m.ChartCategoryId == 1, orderBy: m => m.OrderByDescending(m => m.DateCreated));

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

                    return new HomepageDto
                    {
                        news = news,
                        photos = photos,
                        chart = chartToFrontend
                    };
                }
            );

            return Ok(response);
        }
    }
}