using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class StorageRepository : IRepository<Storage>
    {
        private readonly AppDbContext _appContext;

        public StorageRepository(AppDbContext appDbContext)
        {
            _appContext = appDbContext;
        }

        public async Task Create(Storage item)
        {
            await _appContext.Storages.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Storage storage = await _appContext.Storages.FindAsync(id);

            if (storage != null)
            {
                _appContext.Storages.Remove(storage);
            }
        }

        public async Task<Storage> Get(int id)
        {
            Storage storage = await _appContext.Storages.FindAsync(id);
            storage.Products = _appContext.GoodStorage.Where(g => g.StorageId == id).ToList();

            return storage;
        }

        public IEnumerable<Storage> GetAll()
        {
            return _appContext.Storages;
        }

        public void Update(Storage item)
        {
            _appContext.Entry(item).State = EntityState.Modified;
        }
    }
}
