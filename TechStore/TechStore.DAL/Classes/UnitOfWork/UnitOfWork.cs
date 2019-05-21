using System;
using System.Threading.Tasks;
using TechStore.DAL.Classes.UnitOfWork.Classes;
using TechStore.DAL.Models;

namespace TechStore.DAL.Classes.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _applicationContext;

        private CustomerRepository _customerRepository;
        private GoodRepository _goodRepository;
        private StorageRepository _storageRepository;
        private OrderRepository _orderRepository;
        private ProducerRepository _producerRepository;
        private ReviewRepository _reviewRepository;
        private CategoryRepository _categoryRepository;

        private bool _disposed = false;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _applicationContext = appDbContext;
        }

        public CustomerRepository Customers => _customerRepository ?? 
                                               (_customerRepository = new CustomerRepository(_applicationContext));

        public ProducerRepository Producers => _producerRepository ?? 
                                               (_producerRepository = new ProducerRepository(_applicationContext));

        public GoodRepository Goods => _goodRepository ?? 
                                       (_goodRepository = new GoodRepository(_applicationContext));

        public StorageRepository Storages => _storageRepository ?? 
                                             (_storageRepository = new StorageRepository(_applicationContext));

        public OrderRepository Orders => _orderRepository ?? 
                                         (_orderRepository = new OrderRepository(_applicationContext));

        public ReviewRepository Reviews => _reviewRepository ?? 
                                           (_reviewRepository = new ReviewRepository(_applicationContext));

        public CategoryRepository Categories => _categoryRepository ??
                                                (_categoryRepository = new CategoryRepository(_applicationContext));

        public async Task SaveAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _applicationContext.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AppDbContext GetContext()
        {
            return _applicationContext;
        }
    }
}
