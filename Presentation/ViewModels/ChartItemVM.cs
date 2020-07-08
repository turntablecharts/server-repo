namespace Presentation.ViewModels
{
    public class ChartItemVM
    {
         public int Rank { get; set; }

        public string Title { get; set; }
        public string Artiste { get; set; }
         public string ImageUri { get; set; }
        public int LastPosition { get; set; }
       
        public int HighestPosition { get; set; }
        public string MusicLink { get; set; }
    }
}