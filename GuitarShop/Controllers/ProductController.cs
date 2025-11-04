using GuitarShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopContext context;
        public ProductController(ShopContext ctx) => context = ctx;

        // /Product/List/{id?}  -> id is the Category.Name, or "All"
        public IActionResult List(string id = "All")
        {
            var categories = context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryID)
                .ToList();

            var products = id == "All"
                ? context.Products
                         .Include(p => p.Category)
                         .AsNoTracking()
                         .OrderBy(p => p.ProductID)
                         .ToList()
                : context.Products
                         .Include(p => p.Category)
                         .AsNoTracking()
                         .Where(p => p.Category.Name == id)
                         .OrderBy(p => p.ProductID)
                         .ToList();

            var model = new ProductsViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategory = id
            };

            return View(model);   // Views/Product/List.cshtml
        }
    }
}
