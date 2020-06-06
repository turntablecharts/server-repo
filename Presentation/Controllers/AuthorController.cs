using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
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
    }
}