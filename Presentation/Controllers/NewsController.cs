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
        public NewsController(
            IGenericRepository<Log> logRepository,
            IGenericRepository<News> newsDataRepo,
            UserManager<IdentityUser> userManager,
            IGenericRepository<NewsCategory> newsCateogryDataRepo,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<TtcUser> userGenericRepo,
            TtcDbContext db)
        {
            _userGenericRepo = userGenericRepo;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logRepository = logRepository;
            _newsCateogryDataRepo = newsCateogryDataRepo;
            _newsDataRepo = newsDataRepo;
            _db = db;
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
            return Ok(news);
        }

      

        [AllowAnonymous]
        [HttpGet("")]
        public async Task<ActionResult> GetAllNews([FromQuery] int pageNumber=1, [FromQuery] int pageSize=10)
        {
            int toSkip = (pageNumber-1) * pageSize;
            long totalitems = _db.News.Count();
            var results = await _db.News.OrderByDescending(m => m.DateCreated).Skip(toSkip).Take(pageSize)
                .Select(m => new {Id = m.Id, Title = m.Title, DateCreated=m.DateCreated, HeaderImageUri=m.HeaderImageUri, 
                Description = Regex.Replace(m.NewsContent.Substring(0, 255)+"..", @"[^0-9a-zA-Z:,.']+", " ")  })
                .ToListAsync();

            return Ok(new {news = results, totalItems = totalitems, currentPage = pageNumber, pageSize = pageSize});
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var news = _newsDataRepo.GetWithInclude(m => m.Id == id, "ttcUser")
                .Select(m => new {Id = m.Id, Title = m.Title, DateCreated=m.DateCreated, 
                    HeaderImageUri=m.HeaderImageUri, Description =Regex.Replace(m.NewsContent.Substring(0, 255)+"..", @"[^0-9a-zA-Z:,.']+", " ") , NewsContent = m.NewsContent})
                .FirstOrDefault();
            if (news == null) { return NotFound(); }
            return Ok(news);
        }

        [HttpPut("{id}")]
        public IActionResult EditNews([FromRoute] int id, [FromBody] News news)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            news.Id = id;

            var updatedNews = _newsDataRepo.UpdateAsync(news);

            if (updatedNews == null) { return NotFound(); }

           
            return Ok(updatedNews);
        }

        //[Authorize]
        [HttpDelete("{id}")]
        public  async Task<IActionResult> DeleteNews([FromRoute] int id)
        {
            try
            {
                var newsToDelete = await _db.News.FirstOrDefaultAsync(m => m.Id == id);
                _db.News.Remove(newsToDelete);
                await _db.SaveChangesAsync();

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