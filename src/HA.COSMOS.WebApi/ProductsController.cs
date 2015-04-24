using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;


namespace HA.COSMOS.WebApi
{
    public class ProductsController:ApiController
    {
        //[HttpGet]
        public List<Product> GetProductList()
        {
            List<Product> productLst = new List<Product>{
                new Product{ProductID="P01",ProductName="Pen",Quantity=10,Price=12},
                new Product{ProductID="P02",ProductName="Copy",Quantity=12,Price=20},
                new Product{ProductID="P03",ProductName="Pencil",Quantity=15,Price=22},
                new Product{ProductID="P04",ProductName="Eraser",Quantity=20,Price=27}
                                         };
            return productLst;
        }
    }
}
