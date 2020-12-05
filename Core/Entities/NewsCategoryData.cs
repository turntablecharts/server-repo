using System.Collections.Generic;

namespace Core.Entities
{
    public class NewsCategoryData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<NewsData> NewsDatas { get; set; }
    }
}