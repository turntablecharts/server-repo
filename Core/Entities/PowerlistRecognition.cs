using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities;

public class PowerlistRecognition
{
    public int Id {get; set;}
    [StringLength(255)]
    public string Name {get; set;}
    public int PowerlistEditionId {get; set;}
    [ForeignKey("PowerlistEditionId")]
    public PowerlistEdition PowerlistEdition {get; set;}
    
    public int PowerlistCategoryId {get; set;}
    [ForeignKey("PowerlistCategoryId")]
    public PowerlistCategory PowerlistCategory {get; set;}
    
    [StringLength(255)]
    public string Office {get; set;}
    public string Remarks {get; set;}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    [StringLength(1000)]
    public string ImageUrl { get; set; }

    public int Rank {get; set;}
    public string? Comments {get; set;}
    public string? CommentWriter {get; set;}
}
