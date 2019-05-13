using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using TechStore.Controllers;
using TechStore.DAL.Models;

namespace TechStore.NUnit.Tests.Controllers
{
    [TestFixture]
    class CustomerControllerTest
    {
        DbContextOptions<AppDbContext> options;
        private AppDbContext context;
        private CustomerController controller;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database").Options;
            context = new AppDbContext(options);

            var userManager = Substitute.For<UserManager<ApplicationUser>>();
            var signManager = Substitute.For<SignInManager<ApplicationUser>>();
            var roleManager = Substitute.For<RoleManager<IdentityRole>>();
            controller = new CustomerController(userManager, signManager, roleManager, context);
        }

        /*[Test]
        public async Task Create_Customer()
        {
            // Arrange
            var customer = new Customer() { Id = 1, FirstName = "Petro", Phone = "380671209491" };
            var model = new CreateCustomerView() { FirstName = customer.FirstName, Phone = customer.Phone };

            // Act
            await controller.Create(model);

            // Assert
            Assert.AreEqual(1, context.Customers.CountAsync());
            Assert.AreEqual(model.FirstName, context.Customers.FirstAsync().Result.FirstName);
        }*/

        [TearDown]
        public void TearDown()
        {
            var context = new AppDbContext(options);
            context.Storages.RemoveRange(context.Storages);
            context.Customers.RemoveRange(context.Customers);
            context.Goods.RemoveRange(context.Goods);
            context.Producers.RemoveRange(context.Producers);
            context.SaveChanges();
        }
    }
}
