using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnderThirtyController : ControllerBase
{
    private readonly IGenericRepository<UnderThirty> _underThirtyRepository;
    private readonly ICacheService _cacheService;

    private const string ACTIVE_UNDER_THIRTY_CACHE_KEY = "under_thirty_active";
    private const string UNDER_THIRTY_CACHE_KEY_PREFIX = "under_thirty_";

    public UnderThirtyController(
        IGenericRepository<UnderThirty> underThirtyRepository,
        ICacheService cacheService
    )
    {
        _underThirtyRepository = underThirtyRepository;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var result = await _cacheService.GetOrCreateAsync(
            ACTIVE_UNDER_THIRTY_CACHE_KEY,
            async () =>
                (await _underThirtyRepository.GetAllAsync(item => item.IsActive))
                    .OrderBy(item => item.Id)
                    .Select(ToResponse)
                    .ToList()
        );

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = $"{UNDER_THIRTY_CACHE_KEY_PREFIX}{id}";
        var result = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var item = await _underThirtyRepository.GetAsync(value => value.Id == id);
                return item == null ? null : ToResponse(item);
            }
        );

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    private static UnderThirtyResponseDto ToResponse(UnderThirty item)
    {
        return new UnderThirtyResponseDto
        {
            Id = item.Id,
            Image = item.Image,
            Headerquote = item.Headerquote,
            Name = item.Name,
            Age = item.Age,
            Role = item.Role,
            Bio = item.Bio,
            Citation = item.Citation,
            CitationAuthor = item.CitationAuthor,
            DateAdded = item.DateAdded,
            IsActive = item.IsActive,
        };
    }
}
