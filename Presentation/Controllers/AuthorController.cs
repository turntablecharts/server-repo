using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Presentation.Utilities;
using Presentation.ViewModels;

namespace Presentation.Controllers
{

    [ApiController]
    [Route("api/author")]
    public class AuthorController : ControllerBase
    {
        private readonly IGenericRepository<Log> _logRepository;
        private readonly IGenericRepository<Chart> _chartRepository;
        private IGenericRepository<TtcUser> _userGenericRepo;
        private IGenericRepository<SubscribersEmail> _subscribers;

        public AuthorController(


            IGenericRepository<Chart> chartRepository,

            IGenericRepository<Log> logRepository,

            IGenericRepository<PhotoItem> photoRepository,

            IGenericRepository<TtcUser> userGenericRepo,
            IGenericRepository<SubscribersEmail> subscribers

        )
        {


            _userGenericRepo = userGenericRepo;

            _chartRepository = chartRepository;

            _logRepository = logRepository;

            _subscribers = subscribers;

        }

        #region charts
        [HttpPost("chart/upload")]
        public async Task<IActionResult> UploadChart([FromForm] ChartVM input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("description", "invalid form format");
                return BadRequest(ModelState);
            }
            List<ChartItemVM> chartListVM = new List<ChartItemVM>();

            using (var reader = new StreamReader(input.DataCSVFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                chartListVM = csv.GetRecords<ChartItemVM>().ToList();
            }

            var chartList = new List<ChartItem>();
            foreach (var item in chartListVM)
            {
                chartList.Add(new ChartItem
                {
                    Title = item.Title,
                    Artiste = item.Artiste,
                    Rank = item.Rank,
                    ImageUri = item.ImageUri,
                    HighestPosition = int.Parse(item.HighestPosition),
                    LastPosition = int.Parse(item.LastPosition),
                    MusicLink = item.MusicLink
                });
            }
            var chartToAdd = new Chart
            {
                DateCreated = DateTime.Now,
                Week = input.Week,
                ChartItems = (List<ChartItem>)chartList,
                Category = input.ChartCategory,
                Genre = input.ChartGenre,
                HeaderVideoUrl = input.HeaderVideoUrl
            };

            await _chartRepository.AddAsync(chartToAdd);
            //await _chartRepo.AddChart (chartToAdd);

            return Ok(chartToAdd);

        }

        [HttpDelete("chart/delete/{id}/{userEmail}")]
        public IActionResult DeleteChart([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                var chartToDelete = _chartRepository.GetWithInclude(m => m.Id == id, "ChartItems").FirstOrDefault();
                _chartRepository.Delete(chartToDelete);

                _logRepository.AddAsync(new Log
                {
                    Name = userEmail,
                    Event = "Deleted chart with id: " + id,
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
        [HttpGet("chart/{id}")]

        public IActionResult GetOnechart([FromRoute] int id)
        {
            var result = _chartRepository.GetWithInclude(m => m.Id == id, "ChartItems").FirstOrDefault();
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpGet("chart/all")]
        public async Task<IActionResult> GetCharts([FromQuery] int? pageNumber)
        {
            var charts = _chartRepository.GetWithInclude(null, "ChartItems").OrderByDescending(m => m.DateCreated).AsQueryable();
            int pageSize = 10;
            return Ok(await PaginatedList<Chart>.CreateAsync(charts, pageNumber ?? 1, pageSize));
        }

        [AllowAnonymous]
        [HttpGet("chart/category/{category}")]
        public IActionResult GetCharts([FromRoute] string category)
        {
            var result = _chartRepository.GetWithInclude(m => m.Category.Contains(category), "ChartItems").OrderByDescending(m => m.DateCreated);
            List<Chart> charts = new List<Chart>();
            foreach (var item in result)
            {

                var chartToFrontend = new Chart
                {
                    Id = item.Id,
                    DateCreated = item.DateCreated,
                    Week = item.Week,
                    ChartItems = item.ChartItems.OrderBy(m => m.Rank).ToList(),
                    Category = item.Category,
                    Genre = item.Genre,
                    HeaderVideoUrl = item.HeaderVideoUrl
                };

                charts.Add(chartToFrontend);
            }

            //int pageSize = 10;
            //return Ok (await PaginatedList<Chart>.CreateAsync (charts.AsQueryable (), pageNumber ?? 1, pageSize));

            return Ok(charts);
        }

        [AllowAnonymous]
        [HttpGet("chart/latest")]
        public IActionResult GetLatestChart([FromQuery] string category)
        {
            var latest = _chartRepository.GetAll().OrderByDescending(m => m.DateCreated).Where(m => m.Category.Contains(category ?? "Turntable Top 50")).FirstOrDefault();

            var result = _chartRepository.GetWithInclude(m => m.Id == latest.Id, "ChartItems").FirstOrDefault();

            var chartToFrontend = new Chart
            {
                Id = result.Id,
                DateCreated = result.DateCreated,
                Week = result.Week,
                ChartItems = result.ChartItems.OrderBy(m => m.Rank).Take(10).ToList(),
                Category = result.Category,
                Genre = result.Genre,
                HeaderVideoUrl = result.HeaderVideoUrl
            };
            //result.ChartItems.OrderBy(m => m.Rank).ToList();

            return Ok(chartToFrontend);
        }
        #endregion




        [AllowAnonymous]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribersEmail subscriberInfo)
        {
            if (!string.IsNullOrEmpty(subscriberInfo.Email))
            {
                var alreadyExists = _subscribers.GetWithInclude(m => m.Email == subscriberInfo.Email, string.Empty);
                if (alreadyExists == null)
                {
                    var subscriber = await _subscribers.AddAsync(subscriberInfo);
                    return Ok(subscriber);
                }
            }

            return Ok();
        }


        [AllowAnonymous]
        [HttpGet("subscribe-list")]
        public IActionResult GetSubscriptions()
        {
            List<SubscribersList> subscribersLists = new List<SubscribersList>();
            var emails = _subscribers.GetAll().Where(m => !string.IsNullOrEmpty(m.Email)).Select(m => m.Email).Distinct().ToList();
            foreach (var item in emails)
            {
                subscribersLists.Add(new SubscribersList
                {
                    EmailAddress = item
                });
            }

            using (var writer = new StreamWriter("total-emails.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(subscribersLists);
            }

            


            return Ok(subscribersLists);
        }
    }

}