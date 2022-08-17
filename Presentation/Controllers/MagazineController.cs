using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public MagazineController(IGenericRepository<TtcUser> userGenericRepo,
            IGenericRepository<Log> logRepository,
            IGenericRepository<MagazineEditionData> magEditionRepository,
            IGenericRepository<MagazineData> magazineRepository)
        {
            _userGenericRepo = userGenericRepo;
            _logRepository = logRepository;
            _magazineRepository = magazineRepository;
            _magEditionRepository = magEditionRepository;
        }

        #region magazine
        [Authorize(Roles = "Admin, Author, Writer")]
        [HttpPost("magazine/add")]
        public async Task<IActionResult> AddMagazine([FromBody] MagazineVM item)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            item.DateCreated = DateTime.Now;

            var user = _userGenericRepo.GetWithInclude(m => m.Email == item.Email, string.Empty).FirstOrDefault();

            item.TtcUserId = user.Id;

            int magEditionId;

            var edition = _magEditionRepository.GetWithInclude(m => m.Name.ToUpper() == item.Edition.ToUpper(), string.Empty).FirstOrDefault();

            if (edition == null)
            {
                var createdEdition = await _magEditionRepository.AddAsync(new MagazineEditionData
                {
                    Name = item.Edition
                });

                magEditionId = createdEdition.Id;
            }
            else
            {
                magEditionId = edition.Id;
            }


            item.MagazineEditionDataId = magEditionId;

            var magazine = await _magazineRepository.AddAsync(item);

            return Ok(magazine);
        }

        [AllowAnonymous]
        [HttpGet("magazine/all")]
        public async Task<IActionResult> GetAllMagazine([FromQuery] int? pageNumber)
        {
            int pageSize = 20;
            var magazines = _magazineRepository.GetAll().OrderByDescending(m => m.DateCreated);
            return Ok(await PaginatedList<MagazineData>.CreateAsync(magazines, pageNumber ?? 1, pageSize));
        }

        [AllowAnonymous]
        [HttpGet("magazine/{id}")]
        public IActionResult GetOneMagazine([FromRoute] int id)
        {
            var magazine = _magazineRepository.GetById(id);
            if (magazine == null)
            {
                return NotFound();
            }

            var result = new MagazineContentDto
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
                NextArticle = new MagazineData()
            };

            var magazines = _magEditionRepository.GetWithInclude(m => m.Id == magazine.MagazineEditionDataId, "MagazineDatas").FirstOrDefault();           

           var outputOthers = magazines.MagazineDatas.OrderBy(m => m.ArticlePosition)
                .ToList();

            var currentIndex = outputOthers.FindIndex(m => m.Id == magazine.Id);

            if(currentIndex == 0)
            {
                result.NextArticle = outputOthers[currentIndex+1];
            }
            if(currentIndex == outputOthers.Count() -1)
            {
                result.NextArticle = outputOthers[0];
            }
            else 
            {
                result.NextArticle = outputOthers[currentIndex+1];
            }

            result.NextArticle.Content = null;

            return Ok(result);
        }

        [Authorize(Roles = "Admin, Author, Writer")]

        [HttpPut("magazine/edit/{id}/{userEmail}")]
        public async Task<IActionResult> EditMagazine([FromRoute] int id, [FromBody] MagazineData item, [FromRoute] string userEmail)
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
            await _logRepository.AddAsync(new Log
            {
                Name = userEmail,
                EventDate = DateTime.Now,
                Event = "Edited Magazine, title: " + editedMagazine.Title
            });
            return Ok(editedMagazine);
        }

        [Authorize(Roles = "Admin, Author")]
        [HttpDelete("magazine/delete/{id}/{userEmail}")]
        public IActionResult DeleteMagazine([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {

                _magazineRepository.Delete(_magazineRepository.GetById(id));

                _logRepository.AddAsync(new Log
                {
                    Name = userEmail,
                    Event = "Deleted magazine, Id: " + id,
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
        [HttpGet("magazine/edition/{editionName}")]
        public IActionResult GetMagazineByEdition([FromRoute] string editionName)
        {
            var magazines = _magEditionRepository.GetWithInclude(m => m.Name.ToLower() == editionName.ToLower(), "MagazineDatas").FirstOrDefault();

            MagazineEditionData output = new MagazineEditionData();
            var magazineData = magazines.MagazineDatas.Select(m => new {
                Id = m.Id, 
                DateCreated = m.DateCreated,
                Title = m.Title, 
                Writer = m.Description, 
                HeaderImage = m.HeaderImage, 
                magazineEditionDataId = m.MagazineEditionDataId, 
                articlePosition = m.ArticlePosition,
                Description = Regex.Replace(m.Content.Substring(0, 255)+"..", @"[^0-9a-zA-Z:,.']+", " ")})
                .OrderBy(m => m.articlePosition)
                .ToList();

            // output.Id = magazines.Id;
            // output.Name = magazines.Name;
            // output.MagazineDatas = magazineData;

            var result = new {Id = magazines.Id, Name = magazines.Name, magazineData = magazineData};

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("magazine/pages/{editionName}")]
        public IActionResult GetMagazinePages([FromRoute] string editionName)
        {
            var magazines = _magEditionRepository.GetWithInclude(m => m.Name.ToLower() == editionName.ToLower(), "MagazineDatas").FirstOrDefault();
            foreach (var item in magazines.MagazineDatas)
            {
                item.Content = null;
            }

            MagazineEditionData output = new MagazineEditionData();

            output.Id = magazines.Id;
            output.Name = magazines.Name;
            
            output.MagazineDatas = magazines.MagazineDatas.OrderBy(m => m.ArticlePosition)
                .ToList();

            return Ok(output);
        }

        [AllowAnonymous]
        [HttpGet("magazine/editions")]
        public IActionResult GetMagazineEditions()
        {
            var editions = _magEditionRepository.GetAll();
            return Ok(editions);
        }
        #endregion

    }
}