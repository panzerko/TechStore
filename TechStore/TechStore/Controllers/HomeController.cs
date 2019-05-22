using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TechStore.DAL.Classes;
using TechStore.DAL.Classes.UnitOfWork;
using TechStore.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public HomeController(AppDbContext appDbContext)
        {
            this.unitOfWork = new UnitOfWork(appDbContext);
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Categories()
        {
            var categories = unitOfWork.Categories.GetAll().ToList();
            return View(categories);
        }

        public IActionResult Index(int page = 1)
        {
            var filterModel = HttpContext.Session.Get<FindRangeInMainView>("filter");

            if (filterModel == null)
            {
                filterModel = new FindRangeInMainView(unitOfWork);
            }
            else
            {
                filterModel.Categories = filterModel.Categories.Distinct().ToList();
            }

            var goods = HttpContext.Session.Get<List<Good>>("goods");

            if (goods == null)
            {
                goods = unitOfWork.Goods.GetAll().ToList();
            }

            var items = goods.Skip((page - 1) * PageViewModel.PageSize).Take(PageViewModel.PageSize);

            HttpContext.Session.Set("filter", filterModel);
            HttpContext.Session.Set("goods", goods);

            filterModel.Goods = items;
            IndexViewModel model = new IndexViewModel
            {
                PageViewModel = new PageViewModel(goods.Count, page),
                FilterModel = filterModel
            };

            return View(model);
        }

        public IActionResult Filter(IndexViewModel model)
        {
            var goods = new List<Good>();
            var tempModel = new FindRangeInMainView(unitOfWork);
            var allGoods = unitOfWork.Goods.GetAll().ToList();

            model.FilterModel.GoodView.Category = tempModel.Categories.First(t => t.Name == Request.Form["typeSelect"]);
            model.FilterModel.GoodView.Producer = tempModel.Producers.First(t => t.Name == Request.Form["producerSelect"]);

            tempModel.ChoosenType = model.FilterModel.GoodView.Category;
            tempModel.ChoosenProducer = model.FilterModel.GoodView.Producer;

            model.FilterModel.Categories = tempModel.Categories;
            model.FilterModel.Producers = tempModel.Producers;

            foreach (var good in allGoods)
            {
                if (this.AddToResult(model, good))
                {
                    goods.Add(good);
                }
            }

            HttpContext.Session.Set("filter", model.FilterModel);
            HttpContext.Session.Set("goods", goods);

            return RedirectToAction("Index", new { page = 1 });
        }

        public IActionResult TypeSearch(int goodType, string categoryName)
        {
            var model = new FindRangeInMainView(unitOfWork);
            List<Good> goods;
            if (categoryName == "All")
            {
                goods = unitOfWork.Goods.GetAll().ToList();
            }
            else
            {
                goods = unitOfWork.Goods.GetAll().Where(p => p.CategoryId == goodType).ToList();

            }

            HttpContext.Session.Set("filter", model);
            HttpContext.Session.Set("goods", goods);

            return RedirectToAction("Index", new { page = 1 });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Sort()
        {
            var model = HttpContext.Session.Get<FindRangeInMainView>("filter");
            var goods = HttpContext.Session.Get<List<Good>>("goods");

            if (model == null)
            {
                model = new FindRangeInMainView(unitOfWork);
            }

            if (goods == null)
            {
                goods = unitOfWork.Goods.GetAll().ToList();
            }

            Enum.TryParse(Request.Form["sortSelect"].ToString(), out SortBy temp);
            model.SortBy = temp;

            if (Request.Form["sortSelect"] == SortBy.Popularity.ToString())
            {
                goods = goods.OrderBy(g => g.Reviews.Count).ToList();
            }

            if (Request.Form["sortSelect"] == SortBy.PriceFromBigger.ToString())
            {
                goods = goods.OrderByDescending(g => g.Price).ToList();
            }

            if (Request.Form["sortSelect"] == SortBy.PriceFromLower.ToString())
            {
                goods = goods.OrderBy(g => g.Price).ToList();
            }

            model.Goods = goods;
            HttpContext.Session.Set("filter", model);
            HttpContext.Session.Set("goods", goods);

            return RedirectToAction("Index", new { page = 1 });
        }

        private bool AddToResult(IndexViewModel model, Good good)
        {
            bool addToResult = true;

            if (model.FilterModel.GoodView.Name != null && good.Name != model.FilterModel.GoodView.Name)
            {
                addToResult = false;
            }

            if (model.FilterModel.GoodView.YearOfManufacture != null
                && good.YearOfManufacture != model.FilterModel.GoodView.YearOfManufacture)
            {
                addToResult = false;
            }

            if (model.FilterModel.GoodView.Producer != null && good.Producer.Name != model.FilterModel.GoodView.Producer.Name)
            {
                if (model.FilterModel.GoodView.Producer.Name != "All")
                {
                    addToResult = false;
                }
            }


            if (model.FilterModel.GoodView.EndPrice - model.FilterModel.GoodView.StartPrice != 0
                && good.Price < model.FilterModel.GoodView.StartPrice ||
                good.Price > model.FilterModel.GoodView.EndPrice)
            {
                addToResult = false;
            }
           // if (model.FilterModel.GoodView.Type != null && good.Type != model.FilterModel.GoodView.Type)
                if (model.FilterModel.GoodView.Category != null && good.Category.Name != model.FilterModel.GoodView.Category.Name)
            {
                if (model.FilterModel.GoodView.Category.Name != "All")
                {
                    addToResult = false;
                }
            }

            if (model.FilterModel.GoodView.WarrantyTerm != null
                && good.WarrantyTerm != model.FilterModel.GoodView.WarrantyTerm)
            {
                addToResult = false;
            }

            return addToResult;
        }
    }
}
