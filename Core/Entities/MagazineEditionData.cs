using System.Collections.Generic;

namespace Core.Entities
{
    public class MagazineEditionData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MagazineData> MagazineDatas { get; set; }
        public string CoverImageUrl {get; set;}
        public bool IsDelete {get; set;}
    }
}