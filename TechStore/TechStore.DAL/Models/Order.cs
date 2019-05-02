using System;
using System.Collections.Generic;
using TechStore.DAL.Classes;

namespace TechStore.DAL.Models
{
    /// <summary>
    /// Order model for create entity in database
    /// </summary>
    public class Order
    {
        public Order()
        {
            Products = new List<GoodOrder>();
        }

        public int Id { get; set; }

        public ICollection<GoodOrder> Products { get; set; }

        public DateTime OrderDate { get; set; }
        
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string EndPointCity { get; set; }

        public string EndPointStreet { get; set; }

        public int CommonPrice { get; set; }
    }
}