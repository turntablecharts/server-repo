using System;

namespace Core.Entities
{
    public class ChartHighlight
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artiste { get; set; }
         public string ImageUri { get; set; }
        public int LastPosition { get; set; }
       
        public int HighestPosition { get; set; }
        public string MusicLink { get; set; }

        public string ChartHighlightType { get; set; }

        public DateTime DateCreated { get; set; }

        public int Rank { get; set; }
    }
}