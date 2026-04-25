using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Presentation.DTO
{
    public class PowerlistEditionCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PowerlistEditionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class PowerlistCategoryCreateDto
    {
        public List<string> Names { get; set; }
    }

    public class PowerlistCategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class PowerlistRecognitionItemDto
    {
         [Required]
        public string Name { get; set; }
        [Required]
        public int PowerlistEditionId { get; set; }
         [Required]
        public int PowerlistCategoryId { get; set; }
         [Required]
        public string Office { get; set; }
         [Required]
        public string Remarks { get; set; }
         [Required]
        public string ImageUrl { get; set; }
         [Required]
        public int Rank { get; set; }
    }

    public class PowerlistRecognitionBulkCreateDto
    {
        public List<PowerlistRecognitionItemDto> Recognitions { get; set; }
    }

    public class PowerlistRecognitionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PowerlistEditionId { get; set; }
        public int PowerlistCategoryId { get; set; }
        public string Office { get; set; }
        public string Remarks { get; set; }
        public string ImageUrl { get; set; }
        public int Rank { get; set; }
        public bool IsActive { get; set; }
    }
}
