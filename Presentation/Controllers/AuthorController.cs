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

    [Authorize(Roles="Author")]
    [ApiController]
    [Route ("api/[controller]")]
    public class AuthorController : ControllerBase {
        private readonly IChartRepo _chartRepo;
        private readonly INewsRepo _newsRepo;
        private readonly IMediaRepo _mediaRepo;
        private readonly IPhotoRepo _photoRepo;
        private readonly ILogRepo _logRepo;
        private readonly IVideoRepo _videoRepo;
        private IConfiguration _config;

        public AuthorController (
            IChartRepo chartRepo,
            INewsRepo newsRepo,
            IMediaRepo mediaRepo,
            IPhotoRepo photoRepo,
            ILogRepo logRepo,
            IVideoRepo videoRepo,
            IConfiguration config
        ) {
            _chartRepo = chartRepo;
            _newsRepo = newsRepo;
            _mediaRepo = mediaRepo;
            _photoRepo = photoRepo;
            _logRepo = logRepo;
            _videoRepo = videoRepo;
            _config = config;
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

            await _chartRepo.AddChart (chartToAdd);

            return Ok (chartToAdd);
        }

        [HttpDelete ("chart/delete/{id}/{userEmail}")]
        public IActionResult DeleteChart ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _chartRepo.DeleteChart (id);

                _logRepo.AddToLog (new Log {
                    Name = userEmail,
                        Event = "Deleted chart with id: " + id,
                        EventDate = DateTime.Now
                });
                return Ok ("Successfully deleted");
            } catch (NullReferenceException) {
                return NotFound ();
            }
        }

        [HttpGet ("chart/{id}")]
        public async Task<IActionResult> GetOnechart ([FromRoute] int id) {
            var result = await _chartRepo.GetOne (id);
            if (result != null) {
                return Ok (result);
            } else {
                return NotFound ();
            }
        }

        [HttpGet ("chart/all")]
        public async Task<IActionResult> GetCharts () {
            return Ok (await _chartRepo.GetAllCharts ());
        }
        #endregion

        #region news
        [HttpPost ("news/add")]
        public async Task<ActionResult> AddNews ([FromBody] NewsItem news) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }
            news.DateCreated = DateTime.Now;

            await _newsRepo.AddNews (news);
            return Ok (news);
        }

        [HttpGet ("news/all")]
        public async Task<ActionResult> GetAllNews () {
            return Ok (await _newsRepo.GetAllNews ());
        }

        [HttpGet ("news/{id}")]
        public async Task<IActionResult> GetOneNews (int id) {
            var news = await _newsRepo.GetOne (id);
            if (news == null) { return NotFound (); }
            return Ok (news);
        }

        [HttpPut ("news/edit/{id}/{userEmail}")]
        public IActionResult EditNews ([FromRoute] int id, [FromBody] NewsItem news, [FromRoute] string userEmail) {
            if (!ModelState.IsValid) { return BadRequest (ModelState); }

            var updatedNews = _newsRepo.EditNews (news, id);

            if (updatedNews == null) { return NotFound (); }

            _logRepo.AddToLog (new Log {
                Name = userEmail,
                    Event = "Edited News, Title: " + updatedNews.Title,
                    EventDate = DateTime.Now
            });
            return Ok (updatedNews);
        }

        [HttpDelete ("news/delete/{id}/{userEmail}")]
        public IActionResult DeleteNews ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _newsRepo.DeleteNews (id);

                _logRepo.AddToLog (new Log {
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

            await _videoRepo.AddVideo (video);

            return Ok (video);
        }

        [HttpGet ("videos/all")]
        public async Task<IActionResult> GetAllVideos () {
            return Ok (await _videoRepo.GetAllVideos ());
        }

        [HttpGet ("videos/{id}")]
        public async Task<IActionResult> GetOneVideo (int id) {
            var video = await _videoRepo.GetOneVideo (id);
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

            var updatedVideo = _videoRepo.EditVideo (item, id);

            if (updatedVideo == null) {
                return NotFound ();
            }

            _logRepo.AddToLog (new Log {
                Name = userEmail,
                    Event = "Edited Video, title" + updatedVideo.Title,
                    EventDate = DateTime.Now
            });

            return Ok (updatedVideo);
        }

        [HttpDelete ("videos/delete/{id}/{userEmail}")]
        public IActionResult DeleteVideo ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _videoRepo.DeleteVideo (id);

                _logRepo.AddToLog (new Log {
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
            var media = await _mediaRepo.Add (mediaItem, item.Image, blobKey);

            return Ok (media);
        }

        [HttpGet ("media/all")]
        public async Task<IActionResult> GetAllMedia () {
            return Ok (await _mediaRepo.GetAllMedia ());
        }

        [HttpGet ("media/{id}")]
        public async Task<IActionResult> GetOneMedia ([FromRoute] int id) {
            var media = await _mediaRepo.GetOne (id);
            if (media == null) {
                return NotFound ();
            }

            return Ok (media);
        }

        [HttpDelete ("media/delete/{id}/{userEmail}")]
        public IActionResult DeleteMedia ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _mediaRepo.DeleteMedia (id);
                _logRepo.AddToLog (new Log {
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

            var photo = await _photoRepo.AddPhoto (item);

            return Ok (photo);
        }

        [HttpGet ("photo/all")]
        public async Task<IActionResult> GetAllPhoto () {
            return Ok (await _photoRepo.GetAllPhotos ());
        }

        [HttpGet ("photo/{id}")]
        public async Task<IActionResult> GetOnePhoto ([FromRoute] int id) {
            var photo = await _photoRepo.GetOnePhoto (id);
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

            var editedPhoto = _photoRepo.EditPhoto (item, id);

            if (editedPhoto == null) {
                return NotFound ();
            }
            _logRepo.AddToLog (new Log {
                Name = userEmail,
                    EventDate = DateTime.Now,
                    Event = "Edited Video, title: " + editedPhoto.Title
            });
            return Ok (editedPhoto);
        }

        [HttpDelete ("photo/delete/{id}/{userEmail}")]
        public IActionResult DeletePhoto ([FromRoute] int id, [FromRoute] string userEmail) {
            try {
                _photoRepo.DeletePhoto (id);

                _logRepo.AddToLog (new Log {
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