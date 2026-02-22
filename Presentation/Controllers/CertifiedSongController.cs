using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/certified-song")]
    public class CertifiedSongController : ControllerBase
    {
        private IGenericRepository<CertifiedSong> _certifiedSongsRepo;
        private readonly IMemoryCache _cache;

        public CertifiedSongController(
            IGenericRepository<CertifiedSong> certifiedSongsRepo,
            IMemoryCache cache
        )
        {
            _certifiedSongsRepo = certifiedSongsRepo;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cacheKey = "certified_songs_unclaimed";

            if (!_cache.TryGetValue(cacheKey, out List<CertifiedSong> response))
            {
                var data = await _certifiedSongsRepo.GetAllAsync(
                    m => m.IsClaimed == false,
                    orderBy: m => m.OrderByDescending(m => m.CertifiedDate)
                );

                // 👇 force query execution now
                response = data.Take(100).ToList();

                _cache.Set(
                    cacheKey,
                    response,
                    new MemoryCacheEntryOptions
                    {
                        Size = 1,
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                    }
                );
            }

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get([FromQuery] string? query)
        {
            if (string.IsNullOrEmpty(query))
                return await Get();

            query = query.Trim().ToLower();

            var response = await _certifiedSongsRepo.GetAllAsync(
                m =>
                    m.IsClaimed == false
                    && (
                        EF.Functions.Like(m.Title, $"%{query}%")
                        || EF.Functions.Like(m.Artiste, $"%{query}%")
                    ),
                orderBy: m => m.OrderByDescending(m => m.CertifiedDate)
            );
            if (response == null)
                return Ok(new List<CertifiedSong>());
            return Ok(response);
        }
    }
}
