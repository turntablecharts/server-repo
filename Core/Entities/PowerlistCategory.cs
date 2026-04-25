using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities;

public class PowerlistCategory
{
    public int Id { get; set; }
    [StringLength(255)]
    public string Name {get; set;}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<PowerlistRecognition> PowerlistRecognitions { get; set; }
}
