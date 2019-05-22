using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class ProducerRepository : IRepository<Producer>
    {
        private readonly AppDbContext _appContext;

        public ProducerRepository(AppDbContext appDbContext)
        {
            _appContext = appDbContext;
        }

        public async Task Create(Producer item)
        {
            await _appContext.Producers.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Producer producer = await _appContext.Producers.FindAsync(id);

            if (producer != null)
            {
                _appContext.Producers.Remove(producer);
            }
        }

        public async Task<Producer> Get(int id)
        {
            Producer producer = await _appContext.Producers.FindAsync(id);
            producer.Products = _appContext.Goods.Where(g => g.ProducerId == producer.Id).ToList();
            foreach (var good in producer.Products)
            {
                good.Category = _appContext.Categories.First(p => p.Id == good.CategoryId);
            }
            return producer;
        }

        public IEnumerable<Producer> GetAll()
        {
            return _appContext.Producers;   
        }

        public void Update(Producer item)
        {
            _appContext.Entry(item).State = EntityState.Modified;
        }
    }
}
