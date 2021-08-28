namespace Core.Entities {
    public class VideoItem {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUri { get; set; }
        public string Description { get; set; }
        public bool IsToDelete{ get; set; }
        public string Category { get; set; }
    }
}