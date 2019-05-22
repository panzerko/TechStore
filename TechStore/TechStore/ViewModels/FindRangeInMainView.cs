using System.Collections.Generic;
using System.Linq;
using TechStore.DAL.Classes;
using TechStore.DAL.Classes.UnitOfWork;
using TechStore.DAL.Models;

namespace TechStore.ViewModels
{
    public class FindRangeInMainView
    {
        public FindRangeInMainView()
        {
            var category = new Category { Name = "All" };
            Categories = new List<Category> {category};
            var producer = new Producer { Name = "All" };
            Producers = new List<Producer> {producer};
            ChoosenType = category;
            ChoosenProducer = producer;
            AllSort = new Dictionary<string, string>
            {
                { SortBy.PriceFromLower.ToString(), "Lowest price" },
                { SortBy.PriceFromBigger.ToString(), "Highest price" },
                { SortBy.Popularity.ToString(), "Most popular" },
            };
        }

        public FindRangeInMainView(UnitOfWork unitOfWork) : this()
        {
            var allTypes = unitOfWork.Categories.GetAll().ToList();
            var allProducers = unitOfWork.Producers.GetAll().ToList();
            Categories.AddRange(allTypes);
            Producers.AddRange(allProducers);
        }

        public FindGoodView GoodView { get; set; }

        public IEnumerable<Good> Goods { get; set; }

        public List<Category> Categories { get; set; }

        public List<Producer> Producers { get; set; }

        public Category ChoosenType { get; set; }

        public Producer ChoosenProducer { get; set; }

        public SortBy SortBy { get; set; }

        public Dictionary<string, string> AllSort { get; set; }
    }
}
