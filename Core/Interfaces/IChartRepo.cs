using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IChartRepo
    {
        Task<Chart> AddChart(Chart chart);
        void DeleteChart (int id);
        Task<IList<Chart>> GetAllCharts();
        Task<Chart> GetOne(int id);
    }
}