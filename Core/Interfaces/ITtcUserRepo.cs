using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ITtcUserRepo
    {
        Task<TtcUser> Add(TtcUser user);
        TtcUser Edit(TtcUser user, int id);
        Task<TtcUser> GetUser(int id);
        Task<IList<TtcUser>> GetAllUsers();
        void Delete(int id);
    }
}