using System;

namespace Core.Entities
{
    public class MagazineItem
    {
          public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int TtcUserId { get; set; }
        public TtcUser TtcUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public string HeaderImage { get; set; }
    }
}