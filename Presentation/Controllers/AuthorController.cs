using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Presentation.ViewModels;

namespace Presentation.Controllers {

    [Authorize]
    [ApiController]
    [Route ("api/[controller]")]
    public class AuthorController : ControllerBase {

        private readonly IMediaRepo _mediaUpload;
        private IConfiguration _config;

        private readonly IGenericRepository<Chart> _chartRepository;
        private readonly IGenericRepository<NewsItem> _newsRepository;
        private readonly IGenericRepository<Log> _logRepository;
        private readonly IGenericRepository<VideoItem> _videoRepository;
        private readonly IGenericRepository<PhotoItem> _photoRepository;
        private readonly IGenericRepository<MediaItem> _mediaRepository;

        public AuthorController (

            IMediaRepo mediaUpload,

            IConfiguration config,
            IGenericRepository<Chart> chartRepository,
            IGenericRepository<NewsItem> newsRepository,
            IGenericRepository<Log> logRepository,
            IGenericRepository<VideoItem> videoRepository,
            IGenericRepository<PhotoItem> photoRepository,
            IGenericRepository<MediaItem> mediaRepository
        ) {

            _mediaRepository = mediaRepository;
            _mediaUpload = mediaUpload;

            _config = config;
            _chartRepository = chartRepository;
            _newsRepository = newsRepository;
            _logRepository = logRepository;
            _videoRepository = videoRepository;
            _photoRepository = photoRepository;
        }

        #region charts
        [HttpPost ("chart/upload")]
        public async Task<IActionResult> UploadChart ([FromForm] ChartVM input) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }
            var chartList = new List<ChartItem> ();

            using (var reader = new StreamReader (input.DataCSVFile.OpenReadStream ())) {
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine ();
                    var values = line.Split (',');

                    chartList.Add (new ChartItem {
                        Rank = int.Parse (values[0]),
                            Title = values[1].Trim (),
                            Artiste = values[2].Trim (),
                            ImageUri = values[3].Trim (),
                            LastPosition = int.Parse (values[4]),
                            HighestPosition = int.Parse (values[5])
                    });
                }
            }

            var chartToAdd = new Chart {
                DateCreated = DateTime.Now,
                Week = input.Week,
                ChartItems = chartList,
                Category = input.ChartCategory,
                Genre = input.ChartGenre
            };

            await _chartRepository.AddAsync (chartToAdd);
            //await _chartRepo.AddChart (chartToAdd);

            return Ok (chartToAdd);
        }

        [HttpDelete ("chart/delete/{id}/{userEmail}")]
        public IActionResult DeleteChart ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                var chartToDelete = _chartRepository.GetWithInclude (m => m.Id == id, "ChartItems").FirstOrDefault ();
                _chartRepository.Delete (chartToDelete);

                _logRepository.AddAsync (new Log {
                    Name = userEmail,
                        Event = "Deleted chart with id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            } catch (NullReferenceException) {
                return NotFound ();
            }
        }

        [AllowAnonymous]
        [HttpGet ("chart/{id}")]
       
        public IActionResult GetOnechart ([FromRoute] int id) {
            var result = _chartRepository.GetWithInclude (m => m.Id == id, "ChartItems").FirstOrDefault ();
            if (result != null) {
                return Ok (result);
            } else {
                return NotFound ();
            }
        }

        [AllowAnonymous]
        [HttpGet ("chart/all")]
        public IActionResult GetCharts () {
            return Ok (_chartRepository.GetAll ());
        }
        #endregion

        #region news
        [HttpPost ("news/add")]
        public async Task<ActionResult> AddNews ([FromBody] NewsItem news) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }
            news.DateCreated = DateTime.Now;

            await _newsRepository.AddAsync (news);
            return Ok (news);
        }

         [AllowAnonymous]
        [HttpGet ("news/all")]
        public ActionResult GetAllNews () {
            return Ok (_newsRepository.GetAll ().OrderByDescending(m => m.DateCreated));
        }

         [AllowAnonymous]
        [HttpGet ("news/{id}")]
        public IActionResult GetOneNews (int id) {
            var news = _newsRepository.GetById (id);
            if (news == null) { return NotFound (); }
            return Ok (news);
        }

        [HttpPut ("news/edit/{id}/{userEmail}")]
        public IActionResult EditNews ([FromRoute] int id, [FromBody] NewsItem news, [FromRoute] string userEmail) {
            if (!ModelState.IsValid) { return BadRequest (ModelState); }

            news.Id = id;
            var updatedNews = _newsRepository.UpdateAsync (news);

            if (updatedNews == null) { return NotFound (); }

            _logRepository.AddAsync (new Log {
                Name = userEmail,
                    Event = "Edited News, Title: " + updatedNews.Title,
                    EventDate = DateTime.Now
            });
            return Ok (updatedNews);
        }

        [HttpDelete ("news/delete/{id}/{userEmail}")]
        public IActionResult DeleteNews ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _newsRepository.Delete (_newsRepository.GetById (id));

                _logRepository.AddAsync (new Log {
                    Name = userEmail,
                        Event = "Deleted News, id: " + id,
                        EventDate = DateTime.Now
                });

                return Ok ("Successfully deleted");
            } catch (NullReferenceException) {
                return NotFound ();
            }
        }
        #endregion

        #region videos
        [HttpPost ("videos/add")]
        public async Task<IActionResult> AddVideo ([FromBody] VideoItem video) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            var result = await _videoRepository.AddAsync (video);
            return Ok (result);
        }

        [AllowAnonymous]
        [HttpGet ("videos/all")]
        public IActionResult GetAllVideos () {
            return Ok (_videoRepository.GetAll ().OrderByDescending(m=> m.Id));
        }

        [AllowAnonymous]
        [HttpGet ("videos/{id}")]
        public IActionResult GetOneVideo (int id) {
            var video = _videoRepository.GetById (id);
            if (video == null) {
                return NotFound ();
            }

            return Ok (video);
        }

        [HttpPut ("videos/edit/{id}/{userEmail}")]
        public IActionResult EditVideo ([FromRoute] int id, [FromBody] VideoItem item, [FromRoute] string userEmail) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            item.Id = id;
            var updatedVideo = _videoRepository.UpdateAsync (item);

            if (updatedVideo == null) {
                return NotFound ();
            }

            _logRepository.AddAsync (new Log {
                Name = userEmail,
                    Event = "Edited Video, title" + updatedVideo.Title,
                    EventDate = DateTime.Now
            });

            return Ok (updatedVideo);
        }

        [HttpDelete ("videos/delete/{id}/{userEmail}")]
        public IActionResult DeleteVideo ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                var video = _videoRepository.GetById (id);
                _videoRepository.Delete (video);
                _logRepository.AddAsync (new Log {
                    Name = userEmail,
                        Event = "Deleted Video, id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            } catch (NullReferenceException) {
                return NotFound ();
            }
        }
        #endregion

        #region mediaItem
        [HttpPost ("media/add")]
        public async Task<IActionResult> AddMedia ([FromForm] MediaItemVM item) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            var blobKey = _config.GetSection ("BlobSettings").GetValue<String> ("AccessKey");

            var mediaItem = new MediaItem {
                Title = item.Title,
            };
            var media = await _mediaUpload.Add (mediaItem, item.Image, blobKey);

            return Ok (media);
        }

        [HttpGet ("media/all")]
        public IActionResult GetAllMedia () {
            return Ok (_mediaRepository.GetAll ());
        }

        [HttpGet ("media/{id}")]
        public IActionResult GetOneMedia ([FromRoute] int id) {
            var media = _mediaRepository.GetById (id);
            if (media == null) {
                return NotFound ();
            }

            return Ok (media);
        }

        [HttpDelete ("media/delete/{id}/{userEmail}")]
        public IActionResult DeleteMedia ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _mediaRepository.Delete (_mediaRepository.GetById (id));
                _logRepository.AddAsync (new Log {
                    Name = userEmail,
                        Event = "Deleted Media, Id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            } catch (NullReferenceException) {
                return NotFound ();
            }
        }
        #endregion

        #region photo
        [HttpPost ("photo/add")]
        public async Task<IActionResult> AddPhoto ([FromBody] PhotoItem item) {
            if (!ModelState.IsValid) {
                BadRequest (ModelState);
            }
            item.DateCreated = DateTime.Now;

            var photo = await _photoRepository.AddAsync (item);

            return Ok (photo);
        }

        [AllowAnonymous]
        [HttpGet ("photo/all")]
        public IActionResult GetAllPhoto () {
            return Ok (_photoRepository.GetAll ().OrderByDescending(m => m.DateCreated));
        }

        [AllowAnonymous]
        [HttpGet ("photo/{id}")]
        public IActionResult GetOnePhoto ([FromRoute] int id) {
            var photo = _photoRepository.GetById (id);
            if (photo == null) {
                return NotFound ();
            }
            return Ok (photo);
        }

        [HttpPut ("photo/edit/{id}/{userEmail}")]
        public IActionResult EditPhoto ([FromRoute] int id, [FromBody] PhotoItem item, [FromRoute] string userEmail) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }
            item.Id = id;
            var editedPhoto = _photoRepository.UpdateAsync (item);
            if (editedPhoto == null) {
                return NotFound ();
            }
            _logRepository.AddAsync (new Log {
                Name = userEmail,
                    EventDate = DateTime.Now,
                    Event = "Edited Video, title: " + editedPhoto.Title
            });
            return Ok (editedPhoto);
        }

        [HttpDelete ("photo/delete/{id}/{userEmail}")]
        public IActionResult DeletePhoto ([FromRoute] int id, [FromRoute] string userEmail) {
            try {

                _photoRepository.Delete (_photoRepository.GetById (id));

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
        #endregion
    }
}