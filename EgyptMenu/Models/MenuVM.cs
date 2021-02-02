using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EgyptMenu.Models
{
    public class MenuVM
    {
        public decimal CatId { get; set; }
        public string CatName { get; set; }
        public List<ItemVM> CatItems { get; set; }
    }
    public class ItemVM
    {
        public decimal ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemImage { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemAvailable { get; set; }
    }
}