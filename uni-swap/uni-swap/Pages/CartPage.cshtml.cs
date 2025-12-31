using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using uni_swap.Models;

namespace uni_swap.Pages
{
   
    public class CartPageModel : PageModel
    {
        private readonly IConfiguration configuration;
        public List<Product> products;
        public CartPageModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void OnGet()
        {
            products = Product.getCart(HttpContext.Session.GetString("Email"), configuration);
        }
        public void OnPost()
        {
            
            DeleteItem(Convert.ToInt32(Request.Form["productId"]));
        }

        public void DeleteItem(int ProductId)
        {
            Product.DeleteProduct(ProductId, HttpContext.Session.GetString("Email"), configuration);
            OnGet();
        }
    }
}
