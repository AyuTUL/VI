using Microsoft.AspNetCore.Mvc;
using _1_product_list.Models;

namespace _1_product_list.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Golisopod", Price = 80000 },
                new Product { Id = 2, Name = "Kingambit", Price = 1500 },
                new Product { Id = 3, Name = "Dragapult", Price = 2500 },
                new Product { Id = 4, Name = "Grimmsnarl", Price = 25000 }
            };

            return View(products);
        }
    }
}