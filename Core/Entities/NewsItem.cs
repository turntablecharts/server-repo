using System;

namespace Core.Entities {
    public class NewsItem {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int TtcUserId { get; set; }
        public virtual TtcUser ttcUser { get; set; }
        public string HeaderImageUri { get; set; }
        public string NewsContent { get; set; }
        public string Category { get; set; }

        public string Title { get; set; }
        public bool IsToDelete{ get; set; }
    }
}