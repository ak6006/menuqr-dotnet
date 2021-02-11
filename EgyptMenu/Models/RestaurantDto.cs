using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EgyptMenu.Models
{
    public class RestaurantDto
    {
        public decimal id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string name { get; set; }
        public string subdomain { get; set; }
        public string logo { get; set; }
        public string cover { get; set; }
        public short active { get; set; }
        public decimal user_id { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string minimum { get; set; }
        public string description { get; set; }
        public decimal fee { get; set; }
        public decimal static_fee { get; set; }
        public string radius { get; set; }
        public short is_featured { get; set; }
        public int? city_id { get; set; }
    }
}