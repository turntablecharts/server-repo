using System;
using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Presentation.ViewModels
{
    public class ChartVM
    {
        public string Week { get; set; }
        public DateTime DateCreated { get; set; }
        public IFormFile DataCSVFile { get; set; }
        public String  ChartCategory { get; set; }
        public string HeaderVideoUrl { get; set; }
        public int ChartCategoryId {get; set;}
    }
}