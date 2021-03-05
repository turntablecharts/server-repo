using System.Collections.Generic;

namespace Presentation.DTO
{
    public class ChartDto
    {
        public int Id { get; set; }
        public string Week { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string Category { get; set; }
        public string Genre { get; set; }
        public string HeaderVideoUrl{ get; set; }
        public virtual ICollection<ChartItemDto> ChartItems { get; set; }
        public bool IsToDelete{ get; set; }
    }
}