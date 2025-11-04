using GuitarShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuitarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ShopContext context;

        public CategoryController(ShopContext ctx) => context = ctx;

        public IActionResult Index() => RedirectToAction("List");

        // /Admin/Categories/{id?}  where id is the category Name or "All"
        [Route("[area]/Categories/{id?}")]
        public IActionResult List(string id = "All")
        {
            var model = new ProductsViewModel
            {
                Categories = context.Categories
                                          .AsNoTracking()
                                          .OrderBy(c => c.CategoryID)
                                          .ToList(),
                Products = context.Products
                                          .Include(p => p.Category)
                                          .AsNoTracking()
                                          .OrderBy(p => p.ProductID)
                                          .ToList(),
                SelectedCategory = id
            };

            return View(model); // Areas/Admin/Views/Category/List.cshtml
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            return View("AddUpdate", new Category());
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.Action = "Update";
            var category = context.Categories.Find(id);
            return View("AddUpdate", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = category.CategoryID == 0 ? "Add" : "Update";
                return View("AddUpdate", category);
            }

            if (category.CategoryID == 0)
            {
                context.Categories.Add(category);
                TempData["message"] = $"Category '{category.Name}' created.";
            }
            else
            {
                context.Categories.Update(category);
                TempData["message"] = $"Category '{category.Name}' updated.";
            }

            context.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = context.Categories.Find(id) ?? new Category();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            context.Categories.Remove(category);
            context.SaveChanges();
            TempData["message"] = $"Category '{category.Name}' deleted.";
            return RedirectToAction("List");
        }
    }
}
