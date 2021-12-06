using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTO;
using Presentation.Utilities;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/author")]
    public class NewsController : ControllerBase
    {

        private readonly IGenericRepository<Log> _logRepository;
        private readonly IGenericRepository<News> _newsDataRepo;

        private readonly IGenericRepository<NewsCategory> _newsCateogryDataRepo;

        private IGenericRepository<TtcUser> _userGenericRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private IHttpContextAccessor _httpContextAccessor;
        public NewsController(
            IGenericRepository<Log> logRepository,
            IGenericRepository<News> newsDataRepo,
            UserManager<IdentityUser> userManager,
            IGenericRepository<NewsCategory> newsCateogryDataRepo,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<TtcUser> userGenericRepo)
        {
            _userGenericRepo = userGenericRepo;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logRepository = logRepository;
            _newsCateogryDataRepo = newsCateogryDataRepo;
            _newsDataRepo = newsDataRepo;

        }

        #region news
        [HttpPost("news/add")]
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

            int categoryId;

            var newsCategory = _newsCateogryDataRepo.GetWithInclude(m => m.Name == news.Category, string.Empty).FirstOrDefault();

            if (newsCategory == null)
            {
                var createdCategory = await _newsCateogryDataRepo.AddAsync(new NewsCategory
                {
                    Name = news.Category
                });

                categoryId = createdCategory.Id;
            }
            else
            {
                categoryId = newsCategory.Id;
            }

            news.NewsCategoryId = categoryId;

            await _newsDataRepo.AddAsync(news);
            return Ok(news);
        }

        [AllowAnonymous]
        [HttpGet("news/category/{category}")]
        public ActionResult GetNewsByCategory([FromRoute] string category, [FromQuery] int? pageNumber)
        {
            if (pageNumber == null)
            {
                pageNumber = 1;
            }

            int pageSize = 10;
            int skipSize = ((int)pageNumber - 1) * pageSize;

            // var news = _newsCateogryDataRepo.GetWithInclude(m => m.Name == category, "NewsList")
            //     .FirstOrDefault()
            //     .NewsList (m => m.)
            //     .OrderByDescending(m => m.DateCreated)
            //     .Skip(skipSize).Take(pageSize).ToList();

            var categoryDetails = _newsCateogryDataRepo.GetWithInclude(m => m.Name== category, "").FirstOrDefault();
            if(categoryDetails == null)
            {
                return NotFound();
            }
            var news = _newsDataRepo.GetWithInclude(m => m.NewsCategoryId == categoryDetails.Id && m.IsDeleted == false, "")
                    .OrderByDescending(m => m.DateCreated)
                    .Skip(skipSize).Take(pageSize).ToList();


            // if(news.Count() < pageSize)
            // {
            //     news.ToList();
            //     return Ok(news);
            // }
            // else{
            //     news.Skip(skipSize).Take(pageSize).ToList();
            //     return Ok(news);
            // }
            return Ok(news);

        }

        [AllowAnonymous]
        [HttpGet("news/all")]
        public async Task<ActionResult> GetAllNews([FromQuery] int? pageNumber)
        {
            var news = _newsDataRepo.GetAll().Where(m => m.IsDeleted == false).OrderByDescending(m => m.DateCreated);
            int pageSize = 10;
            return Ok(await PaginatedList<News>.CreateAsync(news, pageNumber ?? 1, pageSize));
        }

        [AllowAnonymous]
        [HttpGet("news/{id}")]
        public IActionResult GetOneNews(int id)
        {
            var news = _newsDataRepo.GetWithInclude(m => m.Id == id, "ttcUser").FirstOrDefault();
            if (news == null) { return NotFound(); }
            return Ok(news);
        }

        [HttpPut("news/edit/{id}/{userEmail}")]
        public IActionResult EditNews([FromRoute] int id, [FromBody] News news, [FromRoute] string userEmail)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            news.Id = id;

            int categoryId;

            var newsCategory = _newsCateogryDataRepo.GetWithInclude(m => m.Name == news.Category, string.Empty).FirstOrDefault();

            if (newsCategory == null)
            {
                var createdCategory = _newsCateogryDataRepo.AddAsync(new NewsCategory
                {
                    Name = news.Category
                });

                categoryId = createdCategory.Id;
            }
            else
            {
                categoryId = newsCategory.Id;
            }

            news.NewsCategoryId = categoryId;

            var updatedNews = _newsDataRepo.UpdateAsync(news);

            if (updatedNews == null) { return NotFound(); }

            _logRepository.AddAsync(new Log
            {
                Name = userEmail,
                Event = "Edited News, Title: " + updatedNews.Title,
                EventDate = DateTime.Now
            });
            return Ok(updatedNews);
        }

        [Authorize]
        [HttpDelete("news/delete/{id}/{userEmail}")]
        public  async Task<IActionResult> DeleteNews([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                var newsToDelete = _newsDataRepo.GetById(id);
                newsToDelete.IsDeleted = true;

                _ =  _newsDataRepo.UpdateAsync(newsToDelete);

                await _logRepository.AddAsync(new Log
                {
                    Name = userEmail,
                    Event = "Deleted News, id: " + id,
                    EventDate = DateTime.Now
                });

                return Ok("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }


        [AllowAnonymous]
        [HttpGet("{authorId}/news")]
        public IActionResult GetNewsByAuthor([FromRoute] int authorId)
        {
            var user = _userGenericRepo.GetWithInclude(m => m.Id == authorId, string.Empty);
            if (user == null)
            {
                return NotFound(new ResponseDto<string>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    ResponseMessage = "User not found"
                });
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
            var response = new ResponseDto<object>
            {
                Data = resObj,
                StatusCode = (int)HttpStatusCode.OK,
                ResponseMessage = "Data Request Successful"
            };
            return Ok(response);

        }

        


        #endregion


    }
}