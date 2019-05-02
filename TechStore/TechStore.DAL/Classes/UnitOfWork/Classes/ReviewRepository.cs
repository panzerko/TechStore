using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class ReviewRepository : IRepository<GoodReview>
    {
        private readonly AppDbContext _appContext;

        public ReviewRepository(AppDbContext appDbContext)
        {
            _appContext = appDbContext;
        }

        public async Task Create(GoodReview item)
        {
            await _appContext.Reviews.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            GoodReview review = await _appContext.Reviews.FindAsync(id);

            if (review != null)
            {
                _appContext.Reviews.Remove(review);
            }
        }

        public async Task<GoodReview> Get(int id)
        {
            GoodReview review = await _appContext.Reviews.FindAsync(id);
            review.Customer = _appContext.Customers.FirstOrDefault(c => c.Id == review.CustomerId);

            return review;
        }

        public IEnumerable<GoodReview> GetAll()
        {
            IEnumerable<GoodReview> reviews = _appContext.Reviews;

            foreach (var review in reviews)
            {
                review.Customer = _appContext.Customers.Where(c => c.Id == review.CustomerId).FirstOrDefault();
                review.Good = _appContext.Goods.Where(g => g.Id == review.GoodId).FirstOrDefault();
            }

            return _appContext.Reviews;
        }

        public void Update(GoodReview item)
        {
            _appContext.Entry(item).State = EntityState.Modified;
        }
    }
}
