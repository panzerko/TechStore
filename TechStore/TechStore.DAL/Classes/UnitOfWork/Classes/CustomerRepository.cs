using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Classes.UnitOfWork.Interfaces;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork.Classes
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly AppDbContext _applicationContext;

        public CustomerRepository(AppDbContext appDbContext)
        {
            _applicationContext = appDbContext;
        }

        public async Task Create(Customer item)
        {
            Cart cart = new Cart
            {
                CustomerId = item.Id
            };

            await _applicationContext.Carts.AddAsync(cart);
            item.Cart = cart;
            await _applicationContext.Customers.AddAsync(item);
        }

        public async Task Delete(int id)
        {
            Customer customer = await _applicationContext.Customers.FindAsync(id);

            if (customer != null)
            {
                Cart cart = _applicationContext.Carts.First(c => c.CustomerId == customer.Id);
                _applicationContext.Carts.Remove(cart);
                _applicationContext.Customers.Remove(customer);
            }
        }

        public async Task<Customer> Get(int id)
        {
            Customer customer = await _applicationContext.Customers.FindAsync(id);
            customer.Cart = _applicationContext.Carts.Where(c => c.CustomerId == customer.Id).FirstOrDefault();
            customer.Cart.Goods = _applicationContext.GoodCart.Where(c => c.CartId == customer.Cart.Id).ToList();

            if (customer.Cart == null)
            {
                Cart cart = new Cart
                {
                    CustomerId = customer.Id
                };

                cart.Goods = _applicationContext.GoodCart.Where(c => c.CartId == cart.Id).ToList();
                customer.Cart = cart;

                await _applicationContext.Carts.AddAsync(cart);
                await _applicationContext.SaveChangesAsync();
            }

            return customer;
        }

        public IEnumerable<Customer> GetAll()
        {
            IEnumerable<Customer> customers = _applicationContext.Customers;

            return customers;
        }

        public void Update(Customer item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }

        public void AddToCart(Good good, Customer customer)
        {
            List<GoodCart> cartGoods = customer.Cart.Goods.ToList();
            bool addToCart = true;

            foreach (var cartGood in cartGoods)
            {
                if (cartGood.GoodId == good.Id)
                {
                    addToCart = false;
                }
            }

            if (addToCart)
            {
                customer.Cart.Goods.Add(new GoodCart { Good = good, Cart = customer.Cart });
            }
        }

        public void RemoveFromCart(Good good, Customer customer)
        {
            List<GoodCart> cartGoods = customer.Cart.Goods.ToList();
            bool removeFromCart = false;

            foreach (var cartGood in cartGoods)
            {
                if (cartGood.GoodId == good.Id)
                {
                    removeFromCart = true;
                }
            }

            if (removeFromCart)
            {
                var goodCart = customer.Cart.Goods
                    .Where(g => g.GoodId == good.Id && g.Cart.Id == customer.Cart.Id).FirstOrDefault();

                if(goodCart != null)
                {
                    customer.Cart.Goods.Remove(goodCart);
                }
            }
        }
    }
}
