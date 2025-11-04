namespace GuitarShop.Models
{
    using System.Collections.Generic;

    public class ProductsViewModel
    {
        public List<Category> Categories { get; set; } = new();
        public List<Product> Products { get; set; } = new();

        public string SelectedCategory { get; set; } = string.Empty;

        public string CheckActiveCategory(string category) =>
            category == SelectedCategory ? "active" : "";
    }
}
