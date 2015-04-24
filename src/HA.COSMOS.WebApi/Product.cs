using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HA.COSMOS.WebApi
{
    public class Product
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}