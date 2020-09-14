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
using Presentation.ViewModels;

namespace Presentation.Controllers
{

    [ApiController]
    [Route ("api/[controller]")]
    public class AuthorController : ControllerBase
    {

        private readonly IMediaRepo _mediaUpload;
        private IConfiguration _config;

        private readonly IGenericRepository<Chart> _chartRepository;
        private readonly IGenericRepository<NewsItem> _newsRepository;
        private readonly IGenericRepository<Log> _logRepository;
        private readonly IGenericRepository<VideoItem> _videoRepository;
        private readonly IGenericRepository<PhotoItem> _photoRepository;
        private readonly IGenericRepository<MediaItem> _mediaRepository;
        private IGenericRepository<TtcUser> _userGenericRepo;
        private IGenericRepository<SubscribersEmail> _subscribers;

        public AuthorController (

            IMediaRepo mediaUpload,

            IConfiguration config,
            IGenericRepository<Chart> chartRepository,
            IGenericRepository<NewsItem> newsRepository,
            IGenericRepository<Log> logRepository,
            IGenericRepository<VideoItem> videoRepository,
            IGenericRepository<PhotoItem> photoRepository,
            IGenericRepository<MediaItem> mediaRepository,
            IGenericRepository<TtcUser> userGenericRepo,
            IGenericRepository<SubscribersEmail> subscribers
        )
        {

            _mediaRepository = mediaRepository;
            _mediaUpload = mediaUpload;
            _userGenericRepo = userGenericRepo;
            _config = config;
            _chartRepository = chartRepository;
            _newsRepository = newsRepository;
            _logRepository = logRepository;
            _videoRepository = videoRepository;
            _photoRepository = photoRepository;
            _subscribers = subscribers;
        }

        #region charts
        [HttpPost ("chart/upload")]
        public async Task<IActionResult> UploadChart ([FromForm] ChartVM input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError ("description", "invalid form format");
                return BadRequest (ModelState);
            }
            List<ChartItemVM> chartListVM = new List<ChartItemVM> ();

            using (var reader = new StreamReader (input.DataCSVFile.OpenReadStream ()))
            using (var csv = new CsvReader (reader, CultureInfo.InvariantCulture))
            {
                chartListVM = csv.GetRecords<ChartItemVM> ().ToList ();
            }

            var chartList = new List<ChartItem> ();
            foreach (var item in chartListVM)
            {
                chartList.Add (new ChartItem
                {
                    Title = item.Title,
                        Artiste = item.Artiste,
                        Rank = item.Rank,
                        ImageUri = item.ImageUri,
                        HighestPosition = item.HighestPosition,
                        LastPosition = item.LastPosition,
                        MusicLink = item.MusicLink
                });
            }
            var chartToAdd = new Chart
            {
                DateCreated = DateTime.Now,
                Week = input.Week,
                ChartItems = (List<ChartItem>) chartList,
                Category = input.ChartCategory,
                Genre = input.ChartGenre,
                HeaderVideoUrl = input.HeaderVideoUrl
            };

            await _chartRepository.AddAsync (chartToAdd);
            //await _chartRepo.AddChart (chartToAdd);

            return Ok (chartToAdd);

        }

        [HttpDelete ("chart/delete/{id}/{userEmail}")]
        public IActionResult DeleteChart ([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                var chartToDelete = _chartRepository.GetWithInclude (m => m.Id == id, "ChartItems").FirstOrDefault ();
                _chartRepository.Delete (chartToDelete);

                _logRepository.AddAsync (new Log
                {
                    Name = userEmail,
                        Event = "Deleted chart with id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound ();
            }
        }

        [AllowAnonymous]
        [HttpGet ("chart/{id}")]

        public IActionResult GetOnechart ([FromRoute] int id)
        {
            var result = _chartRepository.GetWithInclude (m => m.Id == id, "ChartItems").FirstOrDefault ();
            if (result != null)
            {
                return Ok (result);
            }
            else
            {
                return NotFound ();
            }
        }

        [AllowAnonymous]
        [HttpGet ("chart/all")]
        public IActionResult GetCharts ()
        {
            return Ok (_chartRepository.GetWithInclude (null, "ChartItems").OrderByDescending (m => m.DateCreated));
        }

        [AllowAnonymous]
        [HttpGet ("chart/category/{category}")]
        public IActionResult GetCharts ([FromRoute] string category)
        {
            var result = _chartRepository.GetWithInclude (m => m.Category.Contains (category), "ChartItems").OrderByDescending (m => m.DateCreated);
            List<Chart> charts = new List<Chart> ();
            foreach (var item in result)
            {

                var chartToFrontend = new Chart
                {
                    Id = item.Id,
                    DateCreated = item.DateCreated,
                    Week = item.Week,
                    ChartItems = item.ChartItems.OrderBy (m => m.Rank).ToList (),
                    Category = item.Category,
                    Genre = item.Genre,
                    HeaderVideoUrl = item.HeaderVideoUrl
                };

                charts.Add (chartToFrontend);
            }

            return Ok (charts);
        }

        [AllowAnonymous]
        [HttpGet ("chart/latest")]
        public IActionResult GetChartForWeek ()
        {
            var latest = _chartRepository.GetAll ().OrderByDescending (m => m.DateCreated).FirstOrDefault ();

            var result = _chartRepository.GetWithInclude (m => m.Id == latest.Id, "ChartItems").FirstOrDefault ();

            return Ok (result);
        }
        #endregion

        #region news
        [AllowAnonymous]
        [HttpPost ("news/add")]
        public async Task<ActionResult> AddNews ([FromBody] NewsItemVM news)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (ModelState);
            }
            news.DateCreated = DateTime.Now;

            var user = _userGenericRepo.GetWithInclude (m => m.Email == news.Email, string.Empty).FirstOrDefault ();

            news.TtcUserId = user.Id;

            await _newsRepository.AddAsync (news);
            return Ok (news);
        }

        [AllowAnonymous]
        [HttpGet ("news/category/{category}")]
        public ActionResult GetNewsByCategory ([FromRoute] string category)
        {
            var news = _newsRepository.GetWithInclude(m => m.Category.Contains(category), null).OrderByDescending (m => m.DateCreated).ToList();

            return Ok(news);
        }

        [AllowAnonymous]
        [HttpGet ("news/all")]
        public ActionResult GetAllNews ()
        {
            return Ok (_newsRepository.GetAll ().OrderByDescending (m => m.DateCreated));
        }

        [AllowAnonymous]
        [HttpGet ("news/{id}")]
        public IActionResult GetOneNews (int id)
        {
            var news = _newsRepository.GetById (id);
            if (news == null) { return NotFound (); }
            return Ok (news);
        }

        [HttpPut ("news/edit/{id}/{userEmail}")]
        public IActionResult EditNews ([FromRoute] int id, [FromBody] NewsItem news, [FromRoute] string userEmail)
        {
            if (!ModelState.IsValid) { return BadRequest (ModelState); }

            news.Id = id;
            var updatedNews = _newsRepository.UpdateAsync (news);

            if (updatedNews == null) { return NotFound (); }

            _logRepository.AddAsync (new Log
            {
                Name = userEmail,
                    Event = "Edited News, Title: " + updatedNews.Title,
                    EventDate = DateTime.Now
            });
            return Ok (updatedNews);
        }

        [HttpGet ("news/mark-to-delete/{id}")]
        public IActionResult MarkToDeleteNews ([FromRoute] int id)
        {
            var news = _newsRepository.GetById (id);
            news.IsToDelete = true;
            _newsRepository.UpdateAsync (news);

            return Ok (news);
        }

        [HttpDelete ("news/delete/{id}/{userEmail}")]
        public IActionResult DeleteNews ([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                _newsRepository.Delete (_newsRepository.GetById (id));

                _logRepository.AddAsync (new Log
                {
                    Name = userEmail,
                        Event = "Deleted News, id: " + id,
                        EventDate = DateTime.Now
                });

                return Ok ("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound ();
            }
        }
        #endregion

        #region videos
        [HttpPost ("videos/add")]
        public async Task<IActionResult> AddVideo ([FromBody] VideoItem video)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (ModelState);
            }

            var result = await _videoRepository.AddAsync (video);
            return Ok (result);
        }

        [AllowAnonymous]
        [HttpGet ("videos/all")]
        public IActionResult GetAllVideos ()
        {
            return Ok (_videoRepository.GetAll ().OrderByDescending (m => m.Id));
        }

        [AllowAnonymous]
        [HttpGet ("videos/{id}")]
        public IActionResult GetOneVideo (int id)
        {
            var video = _videoRepository.GetById (id);
            if (video == null)
            {
                return NotFound ();
            }

            return Ok (video);
        }

        [AllowAnonymous]
        [HttpGet ("videos/category/{category}")]
        public ActionResult GetVideoByCategory ([FromRoute] string category)
        {
            var news = _videoRepository.GetWithInclude(m => m.Category.Contains(category), null).ToList();

            return Ok(news);
        }

        [HttpPut ("videos/edit/{id}/{userEmail}")]
        public IActionResult EditVideo ([FromRoute] int id, [FromBody] VideoItem item, [FromRoute] string userEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (ModelState);
            }

            item.Id = id;
            var updatedVideo = _videoRepository.UpdateAsync (item);

            if (updatedVideo == null)
            {
                return NotFound ();
            }

            _logRepository.AddAsync (new Log
            {
                Name = userEmail,
                    Event = "Edited Video, title" + updatedVideo.Title,
                    EventDate = DateTime.Now
            });

            return Ok (updatedVideo);
        }

        [HttpGet ("video/mark-to-delete/{id}")]
        public IActionResult MarkToDeleteVideo ([FromRoute] int id)
        {
            var video = _videoRepository.GetById (id);
            video.IsToDelete = true;
            _videoRepository.UpdateAsync (video);

            return Ok (video);
        }

        [HttpDelete ("videos/delete/{id}/{userEmail}")]
        public IActionResult DeleteVideo ([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                var video = _videoRepository.GetById (id);
                _videoRepository.Delete (video);
                _logRepository.AddAsync (new Log
                {
                    Name = userEmail,
                        Event = "Deleted Video, id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound ();
            }
        }
        #endregion

        #region mediaItem
        [HttpPost ("media/add")]
        public async Task<IActionResult> AddMedia ([FromForm] MediaItemVM item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (ModelState);
            }

            var blobKey = _config.GetSection ("BlobSettings").GetValue<String> ("AccessKey");

            var mediaItem = new MediaItem
            {
                Title = item.Title,
            };
            var media = await _mediaUpload.Add (mediaItem, item.Image, blobKey);

            return Ok (media);
        }

        [HttpGet ("media/all")]
        public IActionResult GetAllMedia ()
        {
            return Ok (_mediaRepository.GetAll ());
        }

        [HttpGet ("media/{id}")]
        public IActionResult GetOneMedia ([FromRoute] int id)
        {
            var media = _mediaRepository.GetById (id);
            if (media == null)
            {
                return NotFound ();
            }

            return Ok (media);
        }

        [HttpDelete ("media/delete/{id}/{userEmail}")]
        public IActionResult DeleteMedia ([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {
                _mediaRepository.Delete (_mediaRepository.GetById (id));
                _logRepository.AddAsync (new Log
                {
                    Name = userEmail,
                        Event = "Deleted Media, Id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound ();
            }
        }
        #endregion

        #region photo
        [HttpPost ("photo/add")]
        public async Task<IActionResult> AddPhoto ([FromBody] PhotoItemVM item)
        {
            if (!ModelState.IsValid)
            {
                BadRequest (ModelState);
            }
            item.DateCreated = DateTime.Now;

            var user = _userGenericRepo.GetWithInclude (m => m.Email == item.Email, string.Empty).FirstOrDefault ();

            item.TtcUserId = user.Id;

            var photo = await _photoRepository.AddAsync (item);

            return Ok (photo);
        }

        [AllowAnonymous]
        [HttpGet ("photo/all")]
        public IActionResult GetAllPhoto ()
        {
            return Ok (_photoRepository.GetAll ().OrderByDescending (m => m.DateCreated));
        }

        [AllowAnonymous]
        [HttpGet ("photo/{id}")]
        public IActionResult GetOnePhoto ([FromRoute] int id)
        {
            var photo = _photoRepository.GetById (id);
            if (photo == null)
            {
                return NotFound ();
            }
            return Ok (photo);
        }

        [AllowAnonymous]
        [HttpGet ("photo/category/{category}")]
        public ActionResult GetPhotoByCategory ([FromRoute] string category)
        {
            var news = _photoRepository.GetWithInclude(m => m.Category.Contains(category), null).OrderByDescending (m => m.DateCreated).ToList();

            return Ok(news);
        }

        [HttpPut ("photo/edit/{id}/{userEmail}")]
        public IActionResult EditPhoto ([FromRoute] int id, [FromBody] PhotoItem item, [FromRoute] string userEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest (ModelState);
            }
            item.Id = id;
            var editedPhoto = _photoRepository.UpdateAsync (item);
            if (editedPhoto == null)
            {
                return NotFound ();
            }
            _logRepository.AddAsync (new Log
            {
                Name = userEmail,
                    EventDate = DateTime.Now,
                    Event = "Edited Video, title: " + editedPhoto.Title
            });
            return Ok (editedPhoto);
        }

        [HttpDelete ("photo/delete/{id}/{userEmail}")]
        public IActionResult DeletePhoto ([FromRoute] int id, [FromRoute] string userEmail)
        {
            try
            {

                _photoRepository.Delete (_photoRepository.GetById (id));

                _logRepository.AddAsync (new Log
                {
                    Name = userEmail,
                        Event = "Deleted Photo, Id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            }
            catch (NullReferenceException)
            {
                return NotFound ();
            }
        }
        #endregion

        [AllowAnonymous]
        [HttpPost ("subscribe")]
        public async Task<IActionResult> Subscribe ([FromBody] SubscribersEmail subsciberInfo)
        {
            var subscriber = await _subscribers.AddAsync (subsciberInfo);

            return Ok (subscriber);
        }
        
    }
}