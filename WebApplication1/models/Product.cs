using System;

namespace WebApplication1.models
{
    public class Product
    {

        public string id { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }
        public string mpn { get; set; }
        public string gtin { get; set; }
        public decimal price { get; set; }
        public decimal old_price { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public int store_id { get; set; }

    }
}