using TechStore.DAL.Models;

namespace TechStore.ViewModels
{
    public class FindGoodView
    {
        public string Name { get; set; }

        public Producer Producer { get; set; }

        public int? YearOfManufacture { get; set; }

        public decimal? StartPrice { get; set; }

        public decimal? EndPrice { get; set; }

        public Category Category { get; set; }

        public int? WarrantyTerm { get; set; }
    }
}
