using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;
using Presentation.ViewModels;

namespace Presentation.Controllers {

     [ApiController]
    [Route ("api/author")]
    public class MagazineController : ControllerBase {

        private IGenericRepository<TtcUser> _userGenericRepo;
        private readonly IGenericRepository<Log> _logRepository;
        private IGenericRepository<MagazineData> _magazineRepository;

        private IGenericRepository<MagazineEditionData> _magEditionRepository;
        public MagazineController (IGenericRepository<TtcUser> userGenericRepo,
            IGenericRepository<Log> logRepository,
            IGenericRepository<MagazineEditionData> magEditionRepository,
            IGenericRepository<MagazineData> magazineRepository) {
            _userGenericRepo = userGenericRepo;
            _logRepository = logRepository;
            _magazineRepository = magazineRepository;
            _magEditionRepository = magEditionRepository;
        }

        #region magazine
        [HttpPost ("magazine/add")]
        public async Task<IActionResult> AddMagazine ([FromBody] MagazineVM item) {
            if (!ModelState.IsValid) {
                BadRequest (ModelState);
            }
            item.DateCreated = DateTime.Now;

            var user = _userGenericRepo.GetWithInclude (m => m.Email == item.Email, string.Empty).FirstOrDefault ();

            item.TtcUserId = user.Id;

            int magEditionId;

            var edition = _magEditionRepository.GetWithInclude (m => m.Name.ToUpper() == item.Edition.ToUpper(), string.Empty).FirstOrDefault ();

            if (edition == null) {
                var createdEdition = await _magEditionRepository.AddAsync (new MagazineEditionData {
                    Name = item.Edition
                });

                magEditionId = createdEdition.Id;
            }
            else{
                magEditionId = edition.Id;
            }
           

            item.MagazineEditionDataId = magEditionId;

            var magazine = await _magazineRepository.AddAsync (item);

            return Ok (magazine);
        }

        [AllowAnonymous]
        [HttpGet ("magazine/all")]
        public async Task<IActionResult> GetAllMagazine ([FromQuery] int? pageNumber) {
            int pageSize = 10;
            var magazines = _magazineRepository.GetAll ().OrderByDescending (m => m.DateCreated);
            return Ok (await PaginatedList<MagazineData>.CreateAsync (magazines, pageNumber ?? 1, pageSize));
        }

        [AllowAnonymous]
        [HttpGet ("magazine/{id}")]
        public IActionResult GetOneMagazine ([FromRoute] int id) {
            var magazine = _magazineRepository.GetById (id);
            if (magazine == null) {
                return NotFound ();
            }
            return Ok (magazine);
        }

        [HttpPut ("magazine/edit/{id}/{userEmail}")]
        public async Task<IActionResult> EditMagazine ([FromRoute] int id, [FromBody] MagazineVM item, [FromRoute] string userEmail) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }
            item.Id = id;

            int magEditionId;
            var edition = _magEditionRepository.GetWithInclude (m => m.Name.ToUpper() == item.Edition.ToUpper(), string.Empty).FirstOrDefault ();

            if (edition == null) {
                var createdEdition = await _magEditionRepository.AddAsync (new MagazineEditionData {
                    Name = item.Edition
                });

                magEditionId = createdEdition.Id;
            }
            else{
                magEditionId = edition.Id;
            }
            item.MagazineEditionDataId = magEditionId;

            var editedMagazine = _magazineRepository.UpdateAsync (item);
            if (editedMagazine == null) {
                return NotFound ();
            }
            await _logRepository.AddAsync (new Log {
                Name = userEmail,
                    EventDate = DateTime.Now,
                    Event = "Edited Magazine, title: " + editedMagazine.Title
            });
            return Ok (editedMagazine);
        }

        [HttpDelete ("magazine/delete/{id}/{userEmail}")]
        public IActionResult DeleteMagazine ([FromRoute] int id, [FromRoute] string userEmail) {
            try {

                _magazineRepository.Delete (_magazineRepository.GetById (id));

                _logRepository.AddAsync (new Log {
                    Name = userEmail,
                        Event = "Deleted Photo, Id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            } catch (NullReferenceException) {
                return NotFound ();
            }
        }


        [HttpGet("magazine/edition/{editionName}")]
        public IActionResult GetMagazineByEdition([FromRoute] string editionName)
        {
            var magazines = _magEditionRepository.GetWithInclude(m => m.Name.ToLower() == editionName.ToLower(), "MagazineDatas").FirstOrDefault();

            MagazineEditionData output = new MagazineEditionData();

            output.Id = magazines.Id;
            output.Name = magazines.Name;
            output.MagazineDatas = magazines.MagazineDatas.OrderByDescending(m => m.DateCreated).ToList();

            return Ok(output);
        }
        #endregion

    }
}