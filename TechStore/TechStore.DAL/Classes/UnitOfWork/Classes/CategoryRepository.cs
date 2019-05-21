using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly AppDbContext _appContext;

        public CategoryRepository(AppDbContext appDbContext)
        {
            _appContext = appDbContext;
        }

        public async Task Create(Category item)
        {
            await _appContext.Categories.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Category category = await _appContext.Categories.FindAsync(id);

            if (category != null)
            {
                _appContext.Categories.Remove(category);
            }
        }

        public async Task<Category> Get(int id)
        {
            Category category = await _appContext.Categories.FindAsync(id);
            category.Goods = _appContext.Goods.Where(g => g.CategoryId == id).ToList();

            return category;
        }

        public IEnumerable<Category> GetAll()
        {
            IEnumerable<Category> categories = _appContext.Categories;

            foreach (var category in categories)
            {
                category.Goods = _appContext.Goods.Where(g => g.CategoryId == category.Id).ToList();
            }

            return categories;
        }

        public void Update(Category item)
        {
            _appContext.Entry(item).State = EntityState.Modified;
        }
    }
}
