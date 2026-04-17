using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.DTO;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryController : ControllerBase
{
    private readonly IGenericRepository<Gallery> _galleryRepository;
    private readonly TtcDbContext _db;
    private readonly ICacheService _cacheService;

    // Cache key constants
    private const string ALL_GALLERY_CACHE_KEY_PREFIX = "all_gallery_page_";
    private const int CACHE_DURATION_MINUTES = 60; // 1 hour

    public GalleryController(
        IGenericRepository<Gallery> galleryRepository,
        TtcDbContext db,
        ICacheService cacheService
    )
    {
        _galleryRepository = galleryRepository;
        _db = db;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Get paginated galleries from cache with optional galleryType filter
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetGalleries(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? galleryType = null
    )
    {
        // Create cache key based on page number, size, and galleryType filter
        string cacheKey = $"{ALL_GALLERY_CACHE_KEY_PREFIX}{pageNumber}_{pageSize}_{galleryType ?? 0}";

        var response = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                int toSkip = (pageNumber - 1) * pageSize;
                
                // Build base query with optional galleryType filter
                IQueryable<Gallery> query = _db.Galleries;
                if (galleryType.HasValue)
                {
                    query = query.Where(g => (int)g.GalleryType == galleryType.Value);
                }

                long totalItems = await query.CountAsync();
                var results = await query
                    .OrderBy(g => g.Title)
                    .Skip(toSkip)
                    .Take(pageSize)
                    .Select(g => new
                    {
                        g.Id,
                        g.Title,
                        g.Link,
                        g.GalleryType,
                    })
                    .ToListAsync();

                return new
                {
                    galleries = results,
                    totalItems = totalItems,
                    currentPage = pageNumber,
                    pageSize,
                    galleryType = galleryType,
                };
            }
        );

        return Ok(response);
    }

    /// <summary>
    /// Add a new gallery and preload cache with 1-hour TTL
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> AddGallery([FromBody] Gallery gallery)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Add the gallery to the database
        await _galleryRepository.AddAsync(gallery);

        // Preload the cache with the updated galleries
        _ = RemoveCaches();

        return Ok(
            new ResponseDto<Gallery>
            {
                Data = gallery,
                StatusCode = (int)HttpStatusCode.Created,
                ResponseMessage = "Gallery created successfully and cache preloaded",
            }
        );
    }

    /// <summary>
    /// Helper method to preload gallery caches with 1-hour TTL
    /// </summary>
    private async Task RemoveCaches()
    {
        // Preload common page combinations with 1-hour cache duration
        for (int page = 1; page <= 5; page++)
        {
            for (int size = 10; size <= 50; size += 10)
            {
                string cacheKey = $"{ALL_GALLERY_CACHE_KEY_PREFIX}{page}_{size}";

                _cacheService.Remove(cacheKey);
            }
        }
    }
}
