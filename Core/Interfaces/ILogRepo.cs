using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ILogRepo
    {
        void AddToLog(Log logItem);
        Task<IList<Log>> GetLogs();
    }
}