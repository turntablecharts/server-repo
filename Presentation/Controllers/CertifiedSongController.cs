using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/certified-song")]
    public class CertifiedSongController : ControllerBase
    {
        private IGenericRepository<CertifiedSong> _certifiedSongsRepo;
        public CertifiedSongController(IGenericRepository<CertifiedSong> certifiedSongsRepo)
        {
            _certifiedSongsRepo = certifiedSongsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response =await _certifiedSongsRepo.GetAll().ToListAsync();
            return Ok(response);
        }
    }
}