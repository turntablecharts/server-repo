using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces {
    public interface IMediaRepo {
        Task<MediaItem> Add (MediaItem item, IFormFile file, string blobKey);
    }
}