using System;
using System.Collections.Generic;

namespace Core.Entities {
    public class Chart {
        public int Id { get; set; }
        public string Week { get; set; }
        public DateTime DateCreated { get; set; }

        public string Category { get; set; }
        public string Genre { get; set; }
        public string HeaderVideoUrl{ get; set; }
        public virtual ICollection<ChartItem> ChartItems { get; set; }
        public bool IsDeleted { get; set; }

        public int ChartCategoryId {get; set;}

    }
}