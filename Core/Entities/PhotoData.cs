using System;

namespace Core.Entities
{
    public class PhotoData
    {
         public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int TtcUserId { get; set; }
        public TtcUser TtcUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public string HeaderImage { get; set; }
        public bool IsToDelete{ get; set; }
        public string Category { get; set; }
        public int PhotoCategoryDataId  { get; set; }
    }
}