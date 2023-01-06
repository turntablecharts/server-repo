using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Presentation.ViewModels
{
    public class ChartCategoryVM : ChartCategory
    {
        public ChartItemVM TopSong { get; set; }
    }
}