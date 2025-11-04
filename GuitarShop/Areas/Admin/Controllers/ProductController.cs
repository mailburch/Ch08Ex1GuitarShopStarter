using GuitarShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ShopContext context;

        public ProductController(ShopContext ctx)
        {
            context = ctx;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        [Route("[area]/[controller]s/{id?}")]
        public IActionResult List(string id = "All")
        {
            // build ViewModel instead of using ViewBag
            var categories = context.Categories
                .OrderBy(c => c.CategoryID)
                .ToList();

            var products = id == "All"
                ? context.Products.Include(p => p.Category)
                    .OrderBy(p => p.ProductID).ToList()
                : context.Products.Include(p => p.Category)
                    .Where(p => p.Category.Name == id)
                    .OrderBy(p => p.ProductID).ToList();

            var model = new ProductsViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategory = id
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var product = new Product();
            ViewBag.Action = "Add";
            ViewBag.Categories = context.Categories
                .OrderBy(c => c.CategoryID).ToList();
            return View("AddUpdate", product);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductID == id) ?? new Product();

            ViewBag.Action = "Update";
            ViewBag.Categories = context.Categories
                .OrderBy(c => c.CategoryID).ToList();

            return View("AddUpdate", product);
        }

        [HttpPost]
        public IActionResult Update(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ProductID == 0)
                {
                    context.Products.Add(product);
                    TempData["message"] = $"Product '{product.Name}' added.";
                }
                else
                {
                    context.Products.Update(product);
                    TempData["message"] = $"Product '{product.Name}' updated.";
                }
                context.SaveChanges();
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = "Save";
                ViewBag.Categories = context.Categories
                    .OrderBy(c => c.CategoryID).ToList();
                return View("AddUpdate", product);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = context.Products
                .FirstOrDefault(p => p.ProductID == id) ?? new Product();
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(Product product)
        {
            context.Products.Remove(product);
            context.SaveChanges();
            TempData["message"] = $"Product '{product.Name}' deleted.";
            return RedirectToAction("List");
        }
    }
}
