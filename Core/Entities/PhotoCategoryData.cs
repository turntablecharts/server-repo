using System.Collections.Generic;

namespace Core.Entities
{
    public class PhotoCategoryData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PhotoData> PhotoDatas { get; set; }
    }
}