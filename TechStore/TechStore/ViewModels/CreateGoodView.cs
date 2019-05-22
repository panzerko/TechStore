using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechStore.DAL.Models;

namespace TechStore.ViewModels
{
    public class CreateGoodView
    {
        public CreateGoodView()
        {
            Storages = new List<Storage>();
            Producers = new List<Producer>();
            Categories = new List<Category>();
        }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Specification")]
        [DataType(DataType.MultilineText)]
        public string Specification { get; set; }

        [Required]
        [Display(Name = "Photo Url")]
        public string PhotoUrl { get; set; }

        [Required]
        [Display(Name = "Year of goods production")]
        public int YearOfManufacture { get; set; }

        [Required]
        [Display(Name = "The warranty period")]
        public int WarrantyTerm { get; set; }

        [Required]
        [Display(Name = "Producer")]
        public List<Producer> Producers { get; set; }

        [Required]
        [Display(Name = "Category")]
        public List<Category> Categories { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public int Count { get; set; }

        public List<Storage> Storages { get; set; }
    }
}
