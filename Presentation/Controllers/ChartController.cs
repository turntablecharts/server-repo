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
            var categories = await _db.ChartCategories.ToListAsync();
            return Ok(categories);
        }


        [HttpGet("{chartCategoryId}")]
        public async Task<IActionResult> GetChart([FromRoute]int chartCategoryId=1)
        {
            var result = await _db.Charts.Where(m => m.ChartCategoryId == chartCategoryId)
                            .OrderByDescending(p => p.DateCreated)
                            .Include(m => m.ChartItems)
                            .FirstOrDefaultAsync();

            return Ok(result);
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
            var chartToAdd = new Chart
            {
                DateCreated = input.DateCreated.Year != DateTime.Now.Year ? DateTime.Now: input.DateCreated,
                Week = input.Week == null ? "Week of" : input.Week,
                ChartItems = (List<ChartItem>)chartList,
                Category = input.ChartCategory,
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