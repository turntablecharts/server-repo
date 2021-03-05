namespace Presentation.DTO
{
    public class ChartItemDto
    {
        public int Id { get; set; }
        public int Rank { get; set; }

        public string Title { get; set; }
        public string Artiste { get; set; }
         public string ImageUri { get; set; }
        public string LastPosition { get; set; }
       
        public string Peak { get; set; }
        public string MusicLink { get; set; }

        public string Direction { get; set; }
    }
}