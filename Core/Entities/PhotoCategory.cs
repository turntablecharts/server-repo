using System.Collections.Generic;

namespace Core.Entities
{
    public class PhotoCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}