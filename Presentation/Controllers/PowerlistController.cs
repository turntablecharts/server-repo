using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerlistController : ControllerBase
    {
        private readonly IGenericRepository<PowerlistCategory> _categoryRepo;
        private readonly IGenericRepository<PowerlistEdition> _editionRepo;
        private readonly IGenericRepository<PowerlistRecognition> _recognitionRepo;
        private readonly ICacheService _cacheService;

        private const string POWERLIST_CACHE_KEY = "powerlist_main_data";
        private const string POWERLIST_CATEGORY_CACHE_PREFIX = "powerlist_cat_";

        public PowerlistController(
            IGenericRepository<PowerlistCategory> categoryRepo,
            IGenericRepository<PowerlistEdition> editionRepo,
            IGenericRepository<PowerlistRecognition> recognitionRepo,
            ICacheService cacheService
        )
        {
            _categoryRepo = categoryRepo;
            _editionRepo = editionRepo;
            _recognitionRepo = recognitionRepo;
            _cacheService = cacheService;
        }

        private void ClearCache()
        {
            _cacheService.Remove(POWERLIST_CACHE_KEY);
            _cacheService.Remove($"{POWERLIST_CATEGORY_CACHE_PREFIX}0");
            var activeCategories = _categoryRepo
                .GetAll()
                .Where(c => c.IsActive)
                .Select(c => c.Id)
                .ToList();
            foreach (var id in activeCategories)
            {
                _cacheService.Remove($"{POWERLIST_CATEGORY_CACHE_PREFIX}{id}");
            }
        }

        #region Editions

        [HttpPost("edition")]
        public async Task<IActionResult> CreateEdition([FromBody] PowerlistEditionCreateDto dto)
        {
            if (dto == null)
                return BadRequest();

            var edition = new PowerlistEdition
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            var result = await _editionRepo.AddAsync(edition);
            ClearCache();

            return Ok(
                new PowerlistEditionResponseDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Description = result.Description,
                    IsActive = result.IsActive,
                }
            );
        }

        [HttpPut("edition/{id}/toggle-active")]
        public async Task<IActionResult> ToggleEditionActive(int id)
        {
            var edition = _editionRepo.GetById(id);
            if (edition == null)
                return NotFound();

            edition.IsActive = !edition.IsActive;
            edition.UpdatedAt = DateTime.UtcNow;

            _editionRepo.UpdateAsync(edition);
            ClearCache();

            return Ok(
                new PowerlistEditionResponseDto
                {
                    Id = edition.Id,
                    Name = edition.Name,
                    Description = edition.Description,
                    IsActive = edition.IsActive,
                }
            );
        }

        #endregion

        #region Categories

        [HttpPost("category")]
        public async Task<IActionResult> CreateCategories([FromBody] PowerlistCategoryCreateDto dto)
        {
            if (dto == null || dto.Names == null || !dto.Names.Any())
                return BadRequest();

            var existingNames = _categoryRepo.GetAll().Select(c => c.Name.ToLower()).ToList();
            var newCategories = new List<PowerlistCategory>();

            foreach (var name in dto.Names)
            {
                if (!existingNames.Contains(name.Trim().ToLower()))
                {
                    newCategories.Add(
                        new PowerlistCategory
                        {
                            Name = name.Trim(),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = true,
                        }
                    );
                }
            }

            if (newCategories.Any())
            {
                await _categoryRepo.AddRange(newCategories);
                ClearCache();
            }

            var allRequestedCats = _categoryRepo
                .GetAll()
                .Where(c => dto.Names.Contains(c.Name))
                .Select(c => new PowerlistCategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                })
                .ToList();

            return Ok(allRequestedCats);
        }

        [HttpPut("category/{id}/toggle-active")]
        public async Task<IActionResult> ToggleCategoryActive(int id)
        {
            var category = _categoryRepo.GetById(id);
            if (category == null)
                return NotFound();

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            _categoryRepo.UpdateAsync(category);
            ClearCache();

            return Ok(
                new PowerlistCategoryResponseDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsActive = category.IsActive,
                }
            );
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepo
                .GetAll()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id)
                .Select(c => new PowerlistCategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                })
                .ToList();
            return Ok(categories);
        }

        #endregion

        #region Recognitions

        [HttpPost("recognition")]
        public async Task<IActionResult> CreateRecognitions(
            [FromBody] PowerlistRecognitionBulkCreateDto dto
        )
        {
            if (dto == null || dto.Recognitions == null || !dto.Recognitions.Any())
                return BadRequest();

            var newRecognitions = new List<PowerlistRecognition>();

            foreach (var item in dto.Recognitions)
            {
                // Check if name already exists in THIS edition
                var exists = _recognitionRepo
                    .GetAll()
                    .Any(r =>
                        r.PowerlistEditionId == item.PowerlistEditionId
                        && r.Name.ToLower() == item.Name.ToLower()
                    );

                if (!exists)
                {
                    newRecognitions.Add(
                        new PowerlistRecognition
                        {
                            Name = item.Name,
                            PowerlistEditionId = item.PowerlistEditionId,
                            PowerlistCategoryId = item.PowerlistCategoryId,
                            Office = item.Office,
                            Remarks = item.Remarks,
                            ImageUrl = item.ImageUrl,
                            Rank = item.Rank,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = true,
                        }
                    );
                }
            }

            if (newRecognitions.Any())
            {
                await _recognitionRepo.AddRange(newRecognitions);
                ClearCache();
            }

            return Ok(
                new
                {
                    message = $"Processed {dto.Recognitions.Count} items. Created {newRecognitions.Count} new records.",
                }
            );
        }

        [HttpPut("recognition/{id}/toggle-active")]
        public async Task<IActionResult> ToggleRecognitionActive(int id)
        {
            var recognition = _recognitionRepo.GetById(id);
            if (recognition == null)
                return NotFound();

            recognition.IsActive = !recognition.IsActive;
            recognition.UpdatedAt = DateTime.UtcNow;

            _recognitionRepo.UpdateAsync(recognition);
            ClearCache();

            return Ok(
                new PowerlistRecognitionResponseDto
                {
                    Id = recognition.Id,
                    Name = recognition.Name,
                    PowerlistEditionId = recognition.PowerlistEditionId,
                    PowerlistCategoryId = recognition.PowerlistCategoryId,
                    Office = recognition.Office,
                    Remarks = recognition.Remarks,
                    ImageUrl = recognition.ImageUrl,
                    Rank = recognition.Rank,
                    IsActive = recognition.IsActive,
                }
            );
        }

        #endregion

        #region Public API

        [HttpGet]
        public async Task<IActionResult> GetPowerlist([FromQuery] int? categoryId)
        {
            string cacheKey = $"{POWERLIST_CATEGORY_CACHE_PREFIX}{categoryId ?? 0}";

            var result = await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    // 1. Get the latest active edition
                    var latestEdition = _editionRepo
                        .GetAll()
                        .Where(e => e.IsActive)
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefault();

                    if (latestEdition == null)
                        return null;

                    // 2. Get all active categories
                    var categories = _categoryRepo
                        .GetAll()
                        .Where(c => c.IsActive)
                        .OrderBy(c => c.Id)
                        .Select(c => new PowerlistCategoryResponseDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            IsActive = c.IsActive,
                        })
                        .ToList();

                    var recognitions = new List<PowerlistRecognitionResponseDto>();
                    if (categoryId.HasValue)
                    {
                        recognitions = _recognitionRepo
                            .GetAll()
                            .Where(r =>
                                r.PowerlistEditionId == latestEdition.Id
                                && r.PowerlistCategoryId == categoryId.Value
                                && r.IsActive
                            )
                            .OrderBy(r => r.Rank)
                            .ThenBy(r => r.Name)
                            .Select(r => new PowerlistRecognitionResponseDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                PowerlistEditionId = r.PowerlistEditionId,
                                PowerlistCategoryId = r.PowerlistCategoryId,
                                Office = r.Office,
                                Remarks = r.Remarks,
                                ImageUrl = r.ImageUrl,
                                Rank = r.Rank,
                                IsActive = r.IsActive,
                                Comments = r.Comments,
                                CommentWriter = r.CommentWriter,
                            })
                            .ToList();
                    }
                    else
                    {
                        recognitions = _recognitionRepo
                            .GetAll()
                            .Where(r => r.PowerlistEditionId == latestEdition.Id && r.IsActive)
                            .OrderBy(r => r.PowerlistCategoryId)
                            .ThenBy(r => r.Rank)
                            .ThenBy(r => r.Name)
                            .ToList()
                            .Select(
                                (r, index) =>
                                    new PowerlistRecognitionResponseDto
                                    {
                                        Id = r.Id,
                                        Name = r.Name,
                                        PowerlistEditionId = r.PowerlistEditionId,
                                        PowerlistCategoryId = r.PowerlistCategoryId,
                                        Office = r.Office,
                                        Remarks = r.Remarks,
                                        ImageUrl = r.ImageUrl,
                                        Rank = index + 1,
                                        IsActive = r.IsActive,
                                        Comments = r.Comments,
                                        CommentWriter = r.CommentWriter,
                                    }
                            )
                            .ToList();
                    }

                    return new
                    {
                        latestEdition = new PowerlistEditionResponseDto
                        {
                            Id = latestEdition.Id,
                            Name = latestEdition.Name,
                            Description = latestEdition.Description,
                            IsActive = latestEdition.IsActive,
                        },
                        categories,
                        recognitions,
                    };
                }
            );

            if (result == null)
            {
                return NotFound(new { message = "No active powerlist edition found." });
            }

            return Ok(result);
        }

        #endregion
    }
}
