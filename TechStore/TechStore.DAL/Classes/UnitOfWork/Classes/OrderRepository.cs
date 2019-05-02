using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly AppDbContext _appContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _appContext = dbContext;
        }

        public async Task Create(Order item)
        {
            await _appContext.Orders.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            var order = await _appContext.Orders.FindAsync(id);

            if (order != null)
            {
                _appContext.Orders.Remove(order);
            }
        }

        public async Task<Order> Get(int id)
        {
            var order = await _appContext.Orders.FindAsync(id);

            if (order != null)
            {
                order.Products = _appContext.GoodOrder.Where(g => g.OrderId == order.Id).ToList();
                order.Customer = await _appContext.Customers.FindAsync(order.CustomerId);
            }

            return order;
        }

        public IEnumerable<Order> GetAll()
        {
            IEnumerable<Order> orders = _appContext.Orders;

            foreach (var order in orders)
            {
                order.Customer = _appContext.Customers.Where(c => c.Id == order.CustomerId).FirstOrDefault();
            }

            return orders;
        }

        public void Update(Order item)
        {
            _appContext.Entry(item).State = EntityState.Modified;
        }

        public void SetProducts(List<Good> goods, Order order)
        {
            foreach (var good in goods)
            {
                order.Products.Add(new GoodOrder { Order = order, Good = good });
            }
        }
    }
}