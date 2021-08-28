using Microsoft.AspNetCore.Http;

namespace Presentation.ViewModels
{
    public class MediaItemVM
    {
        public IFormFile Image { get; set; }
        public string Title { get; set; }
    }
}