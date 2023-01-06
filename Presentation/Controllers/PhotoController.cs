using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/author")]

    public class PhotoController : ControllerBase
    {
        private readonly IGenericRepository<Log> _logRepository;
        private readonly IGenericRepository<Photo> _photoDataRepository;
        private readonly IGenericRepository<PhotoCategory> _photoCategoryDataRepo;
        private IGenericRepository<TtcUser> _userGenericRepo;
        public PhotoController(
            IGenericRepository<Log> logRepository,
            IGenericRepository<PhotoCategory> photoCategoryDataRepo,
            IGenericRepository<Photo> photoDataRepository,
            IGenericRepository<TtcUser> userGenericRepo)
        {
            _userGenericRepo = userGenericRepo;

            _logRepository = logRepository;
            _photoCategoryDataRepo = photoCategoryDataRepo;
            _photoDataRepository = photoDataRepository;
        }
        #region photo
        [HttpPost("photo/add")]
        public async Task<IActionResult> AddPhoto([FromBody] PhotoItemVM item)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            item.DateCreated = DateTime.Now;

            var user = _userGenericRepo.GetWithInclude(m => m.Email == item.Email, string.Empty).FirstOrDefault();

            item.TtcUserId = user.Id;

            int categoryId;

            var photoCategory = _photoCategoryDataRepo.GetWithInclude(m => m.Name == item.Category, string.Empty).FirstOrDefault();

            if (photoCategory == null)
            {
                var createdCategory = await _photoCategoryDataRepo.AddAsync(new PhotoCategory
                {
                    Name = item.Category
                });

                categoryId = createdCategory.Id;
            }
            else
            {
                categoryId = photoCategory.Id;
            }

            item.PhotoCategoryId = categoryId;

            var photo = await _photoDataRepository.AddAsync(item);

            return Ok(photo);
        }

        [AllowAnonymous]
        [HttpGet("photo/all")]
        public async Task<IActionResult> GetAllPhoto([FromQuery] int? pageNumber)
        {
            int pageSize = 10;
            var photos = _photoDataRepository.GetAll().Where(m => m.IsDeleted == false).OrderByDescending(m => m.DateCreated);

            return Ok(await PaginatedList<Photo>.CreateAsync(photos, pageNumber ?? 1, pageSize));

        }

        [AllowAnonymous]
        [HttpGet("photo/{id}")]
        public IActionResult GetOnePhoto([FromRoute] int id)
        {
            var photo = _photoDataRepository.GetById(id);
            if (photo == null)
            {
                return NotFound();
            }
            return Ok(photo);
        }

        [AllowAnonymous]
        [HttpGet("photo/category/{category}")]
        public ActionResult GetPhotoByCategory([FromRoute] string category, [FromQuery] int? pageNumber)
        {
            if (pageNumber == null)
            {
                pageNumber = 1;
            }

            int pageSize = 10;
            int skipSize = ((int)pageNumber - 1) * pageSize;

            // var photos = _photoCategoryDataRepo.GetWithInclude(m => m.Name == category, "Photos")
            //     .FirstOrDefault()
            //     .Photos
            //     .OrderByDescending(m => m.DateCreated)
            //     .Skip(skipSize).Take(pageSize).ToList();

            var photoCategory = _photoCategoryDataRepo.GetWithInclude(m => m.Name == category, "").FirstOrDefault();
            if(photoCategory == null)
            {
                return NotFound();
            }
            var photos = _photoDataRepository.GetWithInclude(m => m.IsDeleted == false && m.PhotoCategoryId == photoCategory.Id, "" )
                .OrderByDescending(m => m.DateCreated)
                .Skip(skipSize).Take(pageSize).ToList();


            return Ok(photos);

        }

        [HttpPut("photo/edit/{id}/{userEmail}")]
        public IActionResult EditPhoto([FromRoute] int id, [FromBody] Photo item, [FromRoute] string userEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            item.Id = id;
            int categoryId;

            var photoCategory = _photoCategoryDataRepo.GetWithInclude(m => m.Name == item.Category, string.Empty).FirstOrDefault();

            if (photoCategory == null)
            {
                var createdCategory = _photoCategoryDataRepo.AddAsync(new PhotoCategory
                {
                    Name = item.Category
                });

                categoryId = createdCategory.Id;
            }
            else
            {
                categoryId = photoCategory.Id;
            }

            item.PhotoCategoryId = categoryId;

            var editedPhoto = _photoDataRepository.UpdateAsync(item);
            if (editedPhoto == null)
            {
                return NotFound();
            }
            _logRepository.AddAsync(new Log
            {
                Name = userEmail,
                EventDate = DateTime.Now,
                Event = "Edited Video, title: " + editedPhoto.Title
            });
            return Ok(editedPhoto);
        }

        [HttpDelete("photo/delete/{id}/{userEmail}")]
        public IActionResult DeletePhoto([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                var photoToDelete = _photoDataRepository.GetById(id);
                photoToDelete.IsDeleted = true;

              //  _ =  _newsDataRepo.UpdateAsync(newsToDelete);
                _photoDataRepository.UpdateAsync(photoToDelete);

                _logRepository.AddAsync(new Log
                {
                    Name = userEmail,
                    Event = "Deleted Photo, Id: " + id,
                    EventDate = DateTime.Now
                });
                return Ok("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }



        #endregion
    }
}