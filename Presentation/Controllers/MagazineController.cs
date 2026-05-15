using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Presentation.DTO;
using Presentation.Utilities;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/author")]
    public class MagazineController : ControllerBase
    {
        private IGenericRepository<TtcUser> _userGenericRepo;
        private readonly IGenericRepository<Log> _logRepository;
        private IGenericRepository<MagazineData> _magazineRepository;

        private IGenericRepository<MagazineEditionData> _magEditionRepository;
        private readonly ICacheService _cacheService;

        // Cache key constants
        private const string ALL_MAGAZINES_CACHE_KEY_PREFIX = "all_magazines_page_";
        private const string SINGLE_MAGAZINE_CACHE_KEY_PREFIX = "single_magazine_";
        private const string MAGAZINE_BY_EDITION_CACHE_KEY_PREFIX = "magazine_by_edition_";
        private const string MAGAZINE_PAGES_CACHE_KEY_PREFIX = "magazine_pages_";
        private const string MAGAZINE_EDITIONS_CACHE_KEY = "magazine_editions";
        private const int CACHE_DURATION_MINUTES = 30;

        public MagazineController(
            IGenericRepository<TtcUser> userGenericRepo,
            IGenericRepository<Log> logRepository,
            IGenericRepository<MagazineEditionData> magEditionRepository,
            IGenericRepository<MagazineData> magazineRepository,
            ICacheService cacheService
        )
        {
            _userGenericRepo = userGenericRepo;
            _logRepository = logRepository;
            _magazineRepository = magazineRepository;
            _magEditionRepository = magEditionRepository;
            _cacheService = cacheService;
        }

        #region magazine

        [HttpPost("magazine/add")]
        public async Task<IActionResult> AddMagazine([FromBody] MagazineVM item)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            item.DateCreated = DateTime.Now;

            var user = _userGenericRepo
                .GetWithInclude(m => m.Email == item.Email, string.Empty)
                .FirstOrDefault();

            item.TtcUserId = user.Id;

            var magazine = await _magazineRepository.AddAsync(item);

            // Clear all magazine caches to ensure consistency
            ClearCaches();

            return Ok(magazine);
        }

        [AllowAnonymous]
        [HttpGet("magazine/all")]
        public async Task<IActionResult> GetAllMagazine([FromQuery] int? pageNumber)
        {
            int page = pageNumber ?? 1;
            string cacheKey = $"{ALL_MAGAZINES_CACHE_KEY_PREFIX}{page}";

            var result = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    int pageSize = 20;
                    var magazines = _magazineRepository.GetAll().OrderByDescending(m => m.DateCreated);
                    return await PaginatedList<MagazineData>.CreateAsync(magazines, page, pageSize);
                }
            );

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("magazine/{id}")]
        public async Task<IActionResult> GetOneMagazine([FromRoute] int id)
        {
            string cacheKey = $"{SINGLE_MAGAZINE_CACHE_KEY_PREFIX}{id}";

            var result = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var magazine = _magazineRepository.GetById(id);
                    if (magazine == null)
                    {
                        return null;
                    }

                    var content = new MagazineContentDto
                    {
                        Id = magazine.Id,
                        DateCreated = magazine.DateCreated,
                        TtcUser = magazine.TtcUser,
                        TtcUserId = magazine.TtcUserId,
                        Title = magazine.Title,
                        Description = magazine.Description,
                        Content = magazine.Content,
                        HeaderImage = magazine.HeaderImage,
                        MagazineEditionDataId = magazine.MagazineEditionDataId,
                        ArticlePosition = magazine.ArticlePosition,
                        NextArticle = new MagazineData(),
                    };

                    var magazines = _magEditionRepository
                        .GetWithInclude(m => m.Id == magazine.MagazineEditionDataId, "MagazineDatas")
                        .FirstOrDefault();

                    var outputOthers = magazines.MagazineDatas.OrderBy(m => m.ArticlePosition).ToList();
                    var currentIndex = outputOthers.FindIndex(m => m.Id == magazine.Id);

                    return content;
                }
            );

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("magazine/edit/{id}/{userEmail}")]
        public async Task<IActionResult> EditMagazine(
            [FromRoute] int id,
            [FromBody] MagazineData item,
            [FromRoute] string userEmail
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            item.Id = id;

            var editedMagazine = _magazineRepository.UpdateAsync(item);
            if (editedMagazine == null)
            {
                return NotFound();
            }
            await _logRepository.AddAsync(
                new Log
                {
                    Name = userEmail,
                    EventDate = DateTime.Now,
                    Event = "Edited Magazine, title: " + editedMagazine.Title,
                }
            );

            // Invalidate and repopulate related caches
            ClearCaches();

            return Ok(editedMagazine);
        }

        [HttpDelete("magazine/delete/{id}/{userEmail}")]
        public IActionResult DeleteMagazine([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                _magazineRepository.Delete(_magazineRepository.GetById(id));

                _logRepository.AddAsync(
                    new Log
                    {
                        Name = userEmail,
                        Event = "Deleted magazine, Id: " + id,
                        EventDate = DateTime.Now,
                    }
                );

                ClearCaches();

                return Ok("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpGet("magazine/edition/{editionName}")]
        public async Task<IActionResult> GetMagazineByEdition([FromRoute] string editionName)
        {
            string cacheKey = $"{MAGAZINE_BY_EDITION_CACHE_KEY_PREFIX}{editionName.ToLower()}";

            var result = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var magazines = _magEditionRepository
                        .GetWithInclude(m => m.Name.ToLower() == editionName.ToLower(), "MagazineDatas")
                        .FirstOrDefault();

                    if (magazines == null) return null;

                    var magazineData = magazines
                        .MagazineDatas.Select(m => new
                        {
                            m.Id,
                            m.DateCreated,
                            m.Title,
                            Writer = m.Description,
                            m.HeaderImage,
                            magazineEditionDataId = m.MagazineEditionDataId,
                            articlePosition = m.ArticlePosition,
                        })
                        .OrderBy(m => m.articlePosition)
                        .ToList();

                    return new
                    {
                        magazines.Id,
                        magazines.Name,
                        magazineData,
                    };
                }
            );

            if (result == null) return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("magazine/pages/{editionName}")]
        public IActionResult GetMagazinePages([FromRoute] string editionName)
        {
            var magazines = _magEditionRepository
                .GetWithInclude(m => m.Name.ToLower() == editionName.ToLower(), "MagazineDatas")
                .FirstOrDefault();
            foreach (var item in magazines.MagazineDatas)
            {
                item.Content = null;
            }

            MagazineEditionData output = new MagazineEditionData();

            output.Id = magazines.Id;
            output.Name = magazines.Name;

            output.MagazineDatas = magazines.MagazineDatas.OrderBy(m => m.ArticlePosition).ToList();

            return Ok(output);
        }

        [AllowAnonymous]
        [HttpGet("magazine/editions")]
        public IActionResult GetMagazineEditions([FromQuery]bool? isMagazine)
        {
            IOrderedQueryable<MagazineEditionData> editions;
            if(isMagazine == null)
            {
                editions = _magEditionRepository
                    .GetAll()
                    .Where(m => m.IsDelete == false)
                    .OrderByDescending(m => m.Id);
                return Ok(editions);
            }
            editions = _magEditionRepository
                .GetAll()
                .Where(m => m.IsDelete == false && m.IsMagazine == isMagazine)
                .OrderByDescending(m => m.Id);
            return Ok(editions);
        }

        
        private void ClearCaches()
        {
            // We clear the main edition and editions list caches
            _cacheService.Remove(MAGAZINE_EDITIONS_CACHE_KEY);
            
            // For paginated/prefixed keys, we typically remove the most critical ones 
            // or use a versioning strategy if ICacheService supported it.
            // Here we'll clear the first few pages of 'all' magazines.
            for (int i = 1; i <= 5; i++)
            {
                _cacheService.Remove($"{ALL_MAGAZINES_CACHE_KEY_PREFIX}{i}");
            }
        }
        #endregion
    }
}
