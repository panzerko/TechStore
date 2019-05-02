using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes
{
    public class Updater<T> : IUpdater<T>
    {
        private readonly AppDbContext _dbContext;

        public Updater(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(T item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
        }
    }
}
