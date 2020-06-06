using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DAL
{
    public class TtcUserRepo : ITtcUserRepo
    {
        private TtcDbContext _db;
        public TtcUserRepo(TtcDbContext db)
        {
            _db = db;
        }
        public async Task<TtcUser> Add(TtcUser user)
        {
            await _db.TtcUsers.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async void Delete(int id)
        {
             var user = await _db.TtcUsers.FirstOrDefaultAsync(m => m.Id == id);
            _db.TtcUsers.Remove(user);
            await _db.SaveChangesAsync();
        }

        public TtcUser Edit(TtcUser user, int id)
        {
            var oldUser = _db.TtcUsers.AsNoTracking().FirstOrDefault(m => m.Id == id);
            oldUser = user;
            oldUser.Id = id; 

            _db.TtcUsers.Update(oldUser);
            _db.SaveChanges();

            return oldUser;
        }

        public async Task<IList<TtcUser>> GetAllUsers()
        {
            return await _db.TtcUsers.ToListAsync();
        }

        public async Task<TtcUser> GetUser(int id)
        {
            return await _db.TtcUsers.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}