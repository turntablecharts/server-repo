using System.Collections.Generic;
namespace Presentation.DTO
{
    public class ChartResponseDto
    {
        public int ChartId;
        public List<ChartHighlightsDto> ChartHighlights { get; set; }
        public ChartDto ChartDto { get; set; }
    }
}