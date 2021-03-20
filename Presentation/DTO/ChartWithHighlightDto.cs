using System.Collections.Generic;
using Core.Entities;

namespace Presentation.DTO
{
    public class ChartWithHighlightDto
    {
        public int ChartId { get; set; }
        public List<ChartHighlight> ChartHighlights { get; set; }
        public Chart Chart { get; set; }
    }
}