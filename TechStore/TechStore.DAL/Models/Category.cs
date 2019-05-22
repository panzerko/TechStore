using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace TechStore.DAL.Models
{
    public class Category
    {
        public Category()
        {
            Goods = new List<Good>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Good> Goods { get; set; }
    }
}
