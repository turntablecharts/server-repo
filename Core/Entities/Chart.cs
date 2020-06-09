using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Chart
    {
        public int Id { get; set; }
        public string Week { get; set; }
        public DateTime DateCreated { get; set; }

        public Category Category { get; set; }
        public Genre Genre {get; set;}
        public virtual ICollection<ChartItem> ChartItems {get; set;}
    }


    public enum Category{
        top50, poptop100
    }

    public enum Genre{
        hiphop, rap
    }
}