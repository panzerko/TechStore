using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class GoodRepository : IRepository<Good>
    {
        private readonly AppDbContext _appContext;

        public GoodRepository(AppDbContext appDbContext)
        {
            _appContext = appDbContext;
        }

        public async Task Create(Good item)
        {
            await _appContext.Goods.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Good good = await _appContext.Goods.FindAsync(id);

            if (good != null)
            {
                _appContext.Goods.Remove(good);
            }
        }

        public async Task<Good> Get(int id)
        {
            Good good = await _appContext.Goods.FindAsync(id);
            good.Storages = _appContext.GoodStorage.Where(g => g.GoodId == id).ToList();
            good.Reviews = _appContext.Reviews.Where(r => r.GoodId == id).ToList();
            good.Producer = await _appContext.Producers
                .Where(p => p.Id == good.ProducerId).FirstOrDefaultAsync();
            good.Category = await _appContext.Categories.Where(p => p.Id == good.CategoryId).FirstOrDefaultAsync();
            foreach (var review in good.Reviews)
            {
                review.Customer = await _appContext.Customers
                    .Where(c => c.Id == review.CustomerId).FirstOrDefaultAsync();
            }

            return good;
        }

        public IEnumerable<Good> GetAll()
        {
            IEnumerable<Good> goods = _appContext.Goods
                .Include(p => p.Category)
                .Include(p => p.Producer);
            foreach (var good in goods)
            {
                good.Reviews = GetReviews(good.Id);
            }
            return goods;
        }

        public void Update(Good item)
        {
            _appContext.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteGoodFromStorage(int goodId, List<Storage> storages)
        {
            Good good = await Get(goodId);

            for (int i = 0; i < storages.Count; i++)
            {
                var item = good.Storages.Where(g => g.StorageId == storages[i].Id && g.GoodId == goodId).First();

                if (item != null)
                {
                    _appContext.GoodStorage.Remove(item);
                }
            }
        }

        public async Task AddGoodToStorage(int goodId, List<Storage> storages)
        {
            Good good = await Get(goodId);

            for (int i = 0; i < storages.Count; i++)
            {
                _appContext.GoodStorage.Add(new GoodStorage { Good = good, Storage = storages[i] });
            }
        }

        public List<GoodReview> GetReviews(int id)
        {
            List<GoodReview> reviews = _appContext.Reviews.Local.Where(r => r.GoodId == id).ToList();

            return reviews;
        }

        public async Task AddReview(GoodReview review, Good good)
        {
            review.Good = good;
            await _appContext.Reviews.AddAsync(review);
        }

        public async Task<List<Storage>> GetGoodStorages(int goodId)
        {
            List<Storage> storages = new List<Storage>();
            List<GoodStorage> goodStorages = _appContext.GoodStorage.Where(g => g.GoodId == goodId)
                .ToList();

            foreach (var goodStorage in goodStorages)
            {
                storages.Add(await _appContext.Storages.FindAsync(goodStorage.StorageId));
            }

            return storages;
        }
    }
}
