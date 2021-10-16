using System.Collections.Generic;

namespace Core.Entities
{
    public class NewsCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<News> NewsList { get; set; }
    }
}