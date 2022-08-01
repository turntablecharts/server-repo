using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using CsvHelper;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [Route("api/chart")]
    public class ChartController : ControllerBase
    {
        private readonly ILogger<ChartController> _logger;
        private TtcDbContext _db;
        public ChartController(ILogger<ChartController> logger, 
            TtcDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetChartCategories()
        {
            List<ChartCategoryVM> response = new List<ChartCategoryVM>();

            var categories = await _db.ChartCategories.ToListAsync();
           foreach (var category in categories)
           {
                var chartEntry = await _db.Charts.Where(m => m.ChartCategoryId == category.Id)
                                .AsNoTracking()
                                .OrderByDescending(p => p.DateCreated)
                                .Include(m => m.ChartItems)
                                .FirstOrDefaultAsync();
                    var topSong = chartEntry != null ? chartEntry.ChartItems.FirstOrDefault(): null;
                    
                    response.Add(new ChartCategoryVM{
                        Id = category.Id, 
                        Name = category.Name, 
                        Description = category.Description, 
                        Heading = category.Heading,
                        TopSong = topSong == null ? null :new ChartItemVM
                        {
                            Rank = topSong.Rank, 
                            Title = topSong.Title, 
                            Artiste = topSong.Artiste, 
                            ImageUri = topSong.ImageUri, 
                            MusicLink = topSong.MusicLink, 
                            WeeksOnChart = topSong.WeeksOnChart,
                            LastPosition = topSong.LastPosition.ToString(), 
                            ProducedBy = topSong.ProducedBy
                        }
                    });
           }
            return Ok(response);
        }


        [HttpGet("{chartCategoryId}")]
        public async Task<IActionResult> GetChart([FromRoute]int chartCategoryId=1)
        {
            try
            {
                var result = await _db.Charts.Where(m => m.ChartCategoryId == chartCategoryId)
                            .OrderByDescending(p => p.DateCreated)
                            .Include(m => m.ChartItems)
                            .FirstOrDefaultAsync();

                return Ok(result);
            }
            catch (System.Exception)
            {
                return Ok();
            }
        }

        [HttpGet("{chartCategoryId}/{week}")]
        public async Task<IActionResult> GetChart([FromRoute]int week, [FromRoute]int chartCategoryId=1)
        {
            try
            {
                var result = ISOWeek.ToDateTime(DateTime.Now.Year, week, DayOfWeek.Sunday);

                var dates = new List<DateTime>{result};
                for (int i = 1; i < 7; i++)
                {
                    dates.Add(result.AddDays(i).Date);
                }

                var response = await _db.Charts.Where(m => m.ChartCategoryId == chartCategoryId && 
                            dates.Contains(m.DateCreated.Date))
                            .Include(m => m.ChartItems)
                            .FirstOrDefaultAsync();

                return Ok(response);
            }
            catch (System.Exception)
            {
                return Ok();
            }
        }

         [HttpGet("list/{chartCategoryId}")]
        public async Task<IActionResult> GetCharts([FromRoute]int chartCategoryId=1)
        {
            try
            {
                var result = await _db.Charts.Where(m => m.ChartCategoryId == chartCategoryId)
                            .OrderByDescending(p => p.DateCreated)
                            .Take(10).ToListAsync();

                return Ok(result);
            }
            catch (System.Exception)
            {
                return Ok();
            }
          
        }

        [HttpPost("upload")]
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
                    MusicLink = item.MusicLink,
                    WeeksOnChart = item.WeeksOnChart,
                    ProducedBy = item.ProducedBy
                });
            }

            var chartCategoryName = _db.ChartCategories.FirstOrDefault(m => m.Id == input.ChartCategoryId).Name;
            var chartToAdd = new Chart
            {
                DateCreated = input.DateCreated.Year != DateTime.Now.Year ? DateTime.Now: input.DateCreated,
                Week = input.Week == null ? "Week of" : input.Week,
                ChartItems = (List<ChartItem>)chartList,
                Category = chartCategoryName,
                ChartCategoryId = input.ChartCategoryId,
                HeaderVideoUrl = input.HeaderVideoUrl
            };

            await _db.Charts.AddAsync(chartToAdd);
            await _db.SaveChangesAsync();

            return Ok(chartToAdd);

        }

        [HttpDelete("{chartId}")]
        public async Task<IActionResult> DeleteChart([FromRoute]int chartId)
        {
            var chartToRemove = await _db.Charts.Where(m =>m.Id == chartId)
                .Include(m => m.ChartItems).FirstOrDefaultAsync();

            _db.Remove(chartToRemove);
            await _db.SaveChangesAsync();

            return Ok("Chart Removed");
        }
    }
}