using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IChartRepo _chartRepo;
        private readonly INewsRepo _newsRepo;
        private readonly IMediaRepo _mediaRepo;
        private readonly IPhotoRepo _photoRepo;
        private readonly ILogRepo _logRepo;
        private readonly IVideoRepo _videoRepo;


        public AuthorController(
            IChartRepo chartRepo,
            INewsRepo newsRepo,
            IMediaRepo mediaRepo,
            IPhotoRepo photoRepo,
            ILogRepo logRepo,
            IVideoRepo videoRepo
        )
        {
            _chartRepo =  chartRepo; 
            _newsRepo = newsRepo;
           _mediaRepo = mediaRepo;
           _photoRepo = photoRepo;
            _logRepo = logRepo;
            _videoRepo = videoRepo;
        }
        

        #region charts
        [HttpPost("chart/upload")]
        public async Task<IActionResult> UploadChart([FromForm]ChartVM input)
        {
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var chartList = new List<ChartItem>();
            
            using (var reader = new StreamReader(input.DataCSVFile.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    chartList.Add(new ChartItem
                    {
                        Rank= int.Parse(values[0]),
                        Title = values[1].Trim(),
                        Artiste = values[2].Trim(),
                        ImageUri = values[3].Trim(),
                        LastPosition = int.Parse(values[4]),
                        HighestPosition = int.Parse(values[5])
                    });
                }
            }

            var chartToAdd = new Chart
            {
                DateCreated = DateTime.Now,
                Week = input.Week,
                ChartItems = chartList,
                Category = input.ChartCategory,
                Genre = input.ChartGenre
            };

            await _chartRepo.AddChart(chartToAdd);

            return Ok(chartToAdd);
        }

        [HttpDelete("chart/delete/{id}")]
        public IActionResult DeleteChart([FromRoute] int id)
        {
            try
            {
                _chartRepo.DeleteChart(id);
                return Ok("Successfully deleted");
            }
            catch (System.Exception)
            {
                return BadRequest("provide an Id");
            }
        }

        [HttpGet("chart/{id}")]
        public async Task<IActionResult> GetOnechart([FromRoute]int id)
        {
            var result = await _chartRepo.GetOne(id);
            if (result != null)
            {
                return Ok(result);
            }else{
                return NotFound();
            }
        }

        [HttpGet("chart/all")]
        public async Task<IActionResult> GetCharts()
        {
            return Ok(await _chartRepo.GetAllCharts());
        }
        #endregion

        #region news
        [HttpPost("news/add")]
        public async Task<ActionResult> AddNews([FromBody]NewsItem news)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await  _newsRepo.AddNews(news);
            return Ok(news);
        } 

        [HttpGet("news/all")]
        public async Task<ActionResult> GetAllNews()
        {
            return Ok(await _newsRepo.GetAllNews());
        }

        [HttpGet("news/{id}")]
        public async Task<IActionResult> GetOneNews(int id)
        {
            var news = await _newsRepo.GetOne(id);
            if (news == null) { return NotFound(); }
            return Ok(news);
        }

        [HttpPut("news/edit/{id}")]
        public IActionResult EditNews([FromRoute] int id, [FromBody] NewsItem news)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var updatedNews = _newsRepo.EditNews(news, id);

            if (updatedNews == null) { return NotFound(); }

            return Ok(updatedNews);
        }

        [HttpDelete("news/delete/{id}")]
        public IActionResult DeleteNews([FromRoute] int id)
        {
            try
            {
                _newsRepo.DeleteNews(id);
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