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
            this.Types = new List<string>
            {
                "All"
            };

            AllSort = new Dictionary<string, string>
            {
                { SortBy.PriceFromLower.ToString(), "Lowest price" },
                { SortBy.PriceFromBigger.ToString(), "Highest price" },
                { SortBy.Popularity.ToString(), "Most popular" },
            };
        }

        public FindRangeInMainView(UnitOfWork unitOfWork) : this()
        {
            var allTypes = unitOfWork.Goods.GetAll()
                .Select(p => p.Type)
                .Distinct();
            this.Types.AddRange(allTypes);
        }

        public FindGoodView GoodView { get; set; }

        public IEnumerable<Good> Goods { get; set; }

        public List<string> Types { get; set; }

        public string ChoosenType { get; set; }

        public SortBy SortBy { get; set; }

        public Dictionary<string, string> AllSort { get; set; }
    }
}
