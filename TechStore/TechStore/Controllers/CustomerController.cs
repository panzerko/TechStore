using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.DAL.Classes;
using TechStore.DAL.Classes.UnitOfWork;
using TechStore.DAL.Models;
using TechStore.Helpers;
using TechStore.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechStore.Controllers
{
    [Authorize(Roles = "admin")]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly ErrorMessage _errorMessage;

        public CustomerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _unitOfWork = new UnitOfWork(appDbContext);
            _errorMessage = new ErrorMessage();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_unitOfWork.Customers.GetAll().ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.FirstName + model.SecondName,
                    CreateDate = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                Customer customer = new Customer
                {
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Phone = model.Phone,
                    Email = model.Email
                };

                if (_userManager.Users.Where(u => u.Email == model.Email).Count() > 0)
                {
                    ViewBag.Message = _errorMessage.ReturnErrorMessage("ErrorMessages", "EmailExistAlready");

                    return View("ErrorPage");
                }

                var addResult = await _userManager.CreateAsync(user, model.Password);

                if (addResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "customer");
                    await _unitOfWork.Customers.Create(customer);
                    await _unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Customer customer = await _unitOfWork.Customers.Get(id);
            ApplicationUser user = await _userManager.FindByEmailAsync(customer.Email);

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            if (customer == null)
            {
                return NotFound();
            }

            EditCustomerView model = new EditCustomerView
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                SecondName = customer.SecondName,
                Phone = customer.Phone,
                Email = customer.Email,
                AllRoles = allRoles,
                UserRoles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCustomerView model, List<string> roles)
        {
            if (ModelState.IsValid)
            {
                Customer customer = await _unitOfWork.Customers.Get(model.Id);
                ApplicationUser user = await _userManager.FindByEmailAsync(customer.Email);
                var invalidPhone = false;
                foreach (var number in model.Phone)
                {
                    if (char.IsLetter(number))
                    {
                        invalidPhone = true;
                    }
                }

                if (model.Phone.Length != 10 || model.Phone.First() != '0' || invalidPhone)
                {
                    ViewBag.Message = "Phone number is invalid!";
                    return View("ErrorPhonePage");
                }
                if (user != null && customer != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.UpdateTime = DateTime.Now;

                    var userRoles = await _userManager.GetRolesAsync(user);
                    var allRoles = _roleManager.Roles.ToList();

                    var addedRoles = roles.Except(userRoles);
                    var removedRoles = userRoles.Except(roles);

                    await _userManager.AddToRolesAsync(user, addedRoles);
                    await _userManager.RemoveFromRolesAsync(user, removedRoles);

                    customer.FirstName = model.FirstName;
                    customer.SecondName = model.SecondName;
                    customer.Phone = model.Phone;
                    customer.Email = model.Email;

                    _unitOfWork.Customers.Update(customer);
                    await _unitOfWork.SaveAsync();

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Customer customer = await _unitOfWork.Customers.Get(id);
            ApplicationUser user = await _userManager.FindByEmailAsync(customer.Email);

            if (user != null && customer != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                await _unitOfWork.Customers.Delete(id);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Find(FindCustomerView model)
        {
            List<Customer> customers = new List<Customer>();

            if (ModelState.IsValid)
            {
                var allCustomers = _unitOfWork.Customers.GetAll().ToList();

                foreach (var customer in allCustomers)
                {
                    if (AddToList(model, customer))
                    {
                        customers.Add(customer);
                    }
                }

                HttpContext.Session.Set("list", customers);

                return RedirectToAction("FindResult", "Customer");
            }

            return View(model);
        }

        public IActionResult FindResult()
        {
            var customers = HttpContext.Session.Get<List<Customer>>("list");

            if (customers == null)
            {
                return RedirectToAction("Find");
            }

            return View(customers);
        }

        [HttpGet]
        public async Task<ActionResult> ShowOrders(int id)
        {
            Customer customer = await _unitOfWork.Customers.Get(id);
            List<Order> customerOrders = new List<Order>();

            customerOrders.AddRange(_unitOfWork.Orders.GetAll().Where(o => o.CustomerId == customer.Id));

            return View(customerOrders);
        }

        private bool AddToList(FindCustomerView model, Customer customer)
        {
            bool addToResult = true;

            if (model.FirstName == null && model.SecondName == null &&
            model.Phone == null && model.Email == null)
            {
                addToResult = false;
            }

            if (model.FirstName != null && customer.FirstName != model.FirstName)
            {
                addToResult = false;
            }

            if (model.SecondName != null && customer.SecondName != model.SecondName)
            {
                addToResult = false;
            }

            if (model.Phone != null && customer.Phone != model.Phone)
            {
                addToResult = false;
            }

            if (model.Email != null && customer.Email != model.Email)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
