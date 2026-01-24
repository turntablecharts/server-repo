using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Presentation.DTO;
using Presentation.Utilities;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {

        private readonly IGenericRepository<Log> _logRepository;
        private readonly IGenericRepository<News> _newsDataRepo;

        private readonly IGenericRepository<NewsCategory> _newsCateogryDataRepo;

        private IGenericRepository<TtcUser> _userGenericRepo;

        private TtcDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;

        // Cache key constants
        private const string ALL_NEWS_CACHE_KEY_PREFIX = "all_news_page_";
        private const string NEWS_BY_ID_CACHE_KEY_PREFIX = "news_by_id_";
        private const string NEWS_BY_AUTHOR_CACHE_KEY_PREFIX = "news_by_author_";
        private const int CACHE_DURATION_MINUTES = 30;

        public NewsController(
            IGenericRepository<Log> logRepository,
            IGenericRepository<News> newsDataRepo,
            UserManager<IdentityUser> userManager,
            IGenericRepository<NewsCategory> newsCateogryDataRepo,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<TtcUser> userGenericRepo,
            TtcDbContext db,
            ICacheService cacheService)
        {
            _userGenericRepo = userGenericRepo;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logRepository = logRepository;
            _newsCateogryDataRepo = newsCateogryDataRepo;
            _newsDataRepo = newsDataRepo;
            _db = db;
            _cacheService = cacheService;
        }

        #region news
        [HttpPost("")]
        public async Task<ActionResult> AddNews([FromBody] NewsItemVM news)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            news.IsDeleted = false;
            news.DateCreated = DateTime.Now;

            var user = _userGenericRepo.GetWithInclude(m => m.Email == news.Email, string.Empty).FirstOrDefault();
            news.TtcUserId = user.Id;

            await _newsDataRepo.AddAsync(news);

            // Clear and repopulate caches
            await InvalidateAndRepopulateNewsCaches();

            return Ok(news);
        }



        [AllowAnonymous]
        [HttpGet("")]
        public async Task<ActionResult> GetAllNews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Create cache key based on page number and size
            string cacheKey = $"{ALL_NEWS_CACHE_KEY_PREFIX}{pageNumber}_{pageSize}";

            var response = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    int toSkip = (pageNumber - 1) * pageSize;
                    long totalitems = _db.News.Count();
                    var results = await _db.News.OrderByDescending(m => m.DateCreated).Skip(toSkip).Take(pageSize)
                        .Select(m => new
                        {
                            Id = m.Id,
                            Title = m.Title,
                            DateCreated = m.DateCreated,
                            HeaderImageUri = m.HeaderImageUri,
                            Description = Regex.Replace(m.NewsContent.Substring(0, 255) + "..", @"[^0-9a-zA-Z:,.']+", " ")
                        })
                        .ToListAsync();

                    return new { news = results, totalItems = totalitems, currentPage = pageNumber, pageSize = pageSize };
                }
            );

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Create cache key
            string cacheKey = $"{NEWS_BY_ID_CACHE_KEY_PREFIX}{id}";

            var news = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () => _newsDataRepo.GetWithInclude(m => m.Id == id, "ttcUser")
                    .Select(m => new
                    {
                        Id = m.Id,
                        Title = m.Title,
                        DateCreated = m.DateCreated,
                        HeaderImageUri = m.HeaderImageUri,
                        Description = Regex.Replace(m.NewsContent.Substring(0, 255) + "..", @"[^0-9a-zA-Z:,.']+", " "),
                        NewsContent = m.NewsContent,
                        TtcUser = m.ttcUser,
                        Category = m.Category,
                        NewsCategoryId = m.NewsCategoryId
                    })
                    .FirstOrDefault()
            );

            if (news == null) { return NotFound(); }

            return Ok(news);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditNews([FromRoute] int id, [FromBody] News news)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            news.Id = id;

            var updatedNews = _newsDataRepo.UpdateAsync(news);

            if (updatedNews == null) { return NotFound(); }

            // Clear and repopulate caches
            InvalidateSingleNewsCacheAndRepopulate(id);
            await InvalidateAndRepopulateNewsCaches();

            return Ok(updatedNews);
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews([FromRoute] int id)
        {
            try
            {
                var newsToDelete = await _db.News.FirstOrDefaultAsync(m => m.Id == id);
                _db.News.Remove(newsToDelete);
                await _db.SaveChangesAsync();

                // Clear and repopulate caches
                _cacheService.Remove($"{NEWS_BY_ID_CACHE_KEY_PREFIX}{id}");
                await InvalidateAndRepopulateNewsCaches();

                return Ok("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }


        [AllowAnonymous]
        [HttpGet("{authorId}/news")]
        public async Task<IActionResult> GetNewsByAuthor([FromRoute] int authorId)
        {
            // Create cache key
            string cacheKey = $"{NEWS_BY_AUTHOR_CACHE_KEY_PREFIX}{authorId}";

            var response = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var user = _userGenericRepo.GetWithInclude(m => m.Id == authorId, string.Empty);
                    if (user == null || !user.Any())
                    {
                        return null; // Handle not found in controller
                    }

                    var newsByUser = _newsDataRepo.GetWithInclude(m => m.TtcUserId == authorId && m.IsDeleted == false, string.Empty)
                        .Select(m => new
                        {
                            Id = m.Id,
                            Title = m.Title,
                            Description = m.NewsContent.Substring(0, 200) + "...",
                            HeaderImageUrl = m.HeaderImageUri,
                            Category = m.Category,
                            DateCreated = m.DateCreated
                        }).ToList();

                    var resObj = new AuthorResponse
                    {
                        News = newsByUser,
                        UserDetails = user.Select(m => new { Name = m.LastName.ToUpper() + " , " + m.FirstName, Bio = m.Bio, Id = m.Id }).FirstOrDefault()
                    };
                    return new ResponseDto<object>
                    {
                        Data = resObj,
                        StatusCode = (int)HttpStatusCode.OK,
                        ResponseMessage = "Data Request Successful"
                    };
                }
            );

            if (response == null)
            {
                return NotFound(new ResponseDto<string>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    ResponseMessage = "User not found"
                });
            }

            return Ok(response);
        }
        #endregion

        // Helper method to invalidate all news-related caches
        private void InvalidateNewsCaches()
        {
            for (int page = 1; page <= 10; page++)
            {
                for (int size = 10; size <= 50; size += 10)
                {
                    _cacheService.Remove($"{ALL_NEWS_CACHE_KEY_PREFIX}{page}_{size}");
                }
            }
        }

        // Helper method to invalidate and repopulate single news cache
        private void InvalidateSingleNewsCacheAndRepopulate(int newsId)
        {
            _cacheService.Remove($"{NEWS_BY_ID_CACHE_KEY_PREFIX}{newsId}");
            _ = GetById(newsId); // Trigger repopulation
        }

        // Helper method to invalidate and repopulate all news list caches
        private async Task InvalidateAndRepopulateNewsCaches()
        {
            InvalidateNewsCaches();

            // Repopulate common page combinations
            for (int page = 1; page <= 3; page++)
            {
                for (int size = 10; size <= 30; size += 10)
                {
                    await RepopulateNewsListCache(page, size);
                }
            }
        }

        // Helper method to repopulate a specific news list cache
        private async Task RepopulateNewsListCache(int pageNumber, int pageSize)
        {
            await GetAllNews(pageNumber, pageSize);
        }
    }
}