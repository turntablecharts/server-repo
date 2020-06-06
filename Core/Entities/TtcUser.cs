using System.Collections.Generic;

namespace Core.Entities
{
    public class TtcUser
    {
        public int Id { get; set; }
        public string  FirstName { get; set; }  
        public string LastName { get; set; } 
        public string Email { get; set; }
        public virtual ICollection<NewsItem> NewsItems {get; set;}
        public virtual ICollection<PhotoItem> PhotoItems {get; set;}
    }
}