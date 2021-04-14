using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EgyptMenu.Models
{
    public class ResMgmtViewModel
    {

        public decimal id { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantDescription { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantImage { get; set; }
        public string RestaurantCoverImage { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerPhone { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int? ThemeId { get; set; }
        public DateTime? ordertimeEnd { get; set; }
        public DateTime? starttime { get; set; }
        public DateTime? endtime { get; set; }
    }
}