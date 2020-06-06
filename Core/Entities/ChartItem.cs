namespace Core.Entities
{
    public class ChartItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artiste { get; set; }
        public int Rank { get; set; }
        public int LastPosition {get; set;}
        public string ImageUri {get; set;}
        public Category Category { get; set; }
        public Genre Genre {get; set;}
    }

    public enum Category{
        top50, poptop100
    }

    public enum Genre{
        hiphop, rap
    }
}