using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.DAL.Classes;
using TechStore.DAL.Classes.UnitOfWork;
using TechStore.DAL.Models;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public CategoryController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = unitOfWork.Categories.GetAll().ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryView model)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category
                {
                    Name = model.Name,
                    Title = model.Title,
                    ImageURL = model.ImageURL,
                };

                await unitOfWork.Categories.Create(category);
                await unitOfWork.SaveAsync();

                return RedirectToAction("Index", "Category");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await unitOfWork.Categories.Get(id);

            if (category == null)
            {

                return View("ErrorPage");
            }

            EditCategoryView model = new EditCategoryView
            {
                Id = category.Id,
                Name = category.Name,
                Title = category.Title,
                ImageURL = category.ImageURL,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryView model)
        {
            if (ModelState.IsValid)
            {
                Category category = await unitOfWork.Categories.Get(model.Id);

                if (category != null)
                {
                    category.Name = model.Name;
                    category.Title = model.Title;
                    category.ImageURL = model.ImageURL;

                    unitOfWork.Categories.Update(category);
                    await unitOfWork.SaveAsync();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            Category category = await unitOfWork.Categories.Get(id);

            if (category != null)
            {
                await unitOfWork.Categories.Delete(id);
                await unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Find(FindCategoryView model)
        {
            List<Category> categories = new List<Category>();

            if (ModelState.IsValid)
            {
                var allCategories = unitOfWork.Categories.GetAll().ToList();

                foreach (var category in allCategories)
                {
                    if (this.AddToResult(model, category))
                    {
                        categories.Add(category);
                    }
                }

                HttpContext.Session.Set("list", categories);

                return RedirectToAction("FindResult", "Category");
            }

            return View(model);
        }

        public IActionResult FindResult()
        {
            var categories = HttpContext.Session.Get<List<Category>>("list");

            if (categories == null)
            {
                return RedirectToAction("Find");
            }

            return View(categories);
        }

        [HttpGet]
        public async Task<ActionResult> ShowGoods(int id)
        {
            Category category = await unitOfWork.Categories.Get(id);

            ViewBag.CategoryName = category.Name;

            return View(category.Goods);
        }

        private bool AddToResult(FindCategoryView model, Category category)
        {
            bool addToResult = true;

            if (model.Name == null && model.Title == null && model.ImageURL == null)
            {
                addToResult = false;
            }

            if (model.Name != null && category.Name != model.Name)
            {
                addToResult = false;
            }

            if (model.Title != null && category.Title != model.Title)
            {
                addToResult = false;
            }

            if (model.ImageURL != null && category.ImageURL != model.ImageURL)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
