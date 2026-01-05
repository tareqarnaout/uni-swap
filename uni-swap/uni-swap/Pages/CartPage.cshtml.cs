using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using uni_swap.Models;
using Microsoft.Data.SqlClient;

namespace uni_swap.Pages
{
   
    public class CartPageModel : PageModel
    {
        private readonly IConfiguration configuration;
        public List<Product> products;
        public double PriceSum = 0;
        public CartPageModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult OnGet()
        {
            int? loggedIn = HttpContext.Session.GetInt32("Login");
            if (loggedIn == null || loggedIn != 1)
            {
                return RedirectToPage("./Login");
            }

            products = Product.getCart(HttpContext.Session.GetString("Email"), configuration);
            calculateSum();
            return Page();
        }
        
        public IActionResult OnPost()
        {
            // Check if this is a remove action
            if (Request.Form.ContainsKey("productId"))
            {
                DeleteItem(Convert.ToInt32(Request.Form["productId"]));
                return Page();
            }
            
            return Page();
        }

        public IActionResult OnPostBuy()
        {
            int? loggedIn = HttpContext.Session.GetInt32("Login");
            if (loggedIn == null || loggedIn != 1)
            {
                return RedirectToPage("./Login");
            }

            var email = HttpContext.Session.GetString("Email");
            
            // Get cart items
            products = Product.getCart(email, configuration);
            
            if (products == null || products.Count == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToPage();
            }

            // Calculate total
            calculateSum();
            decimal totalWithFee = (decimal)(PriceSum + (PriceSum * 0.05));

            // Get product IDs from cart
            string productIds = GetCartProductIds(email);

            if (string.IsNullOrEmpty(productIds))
            {
                TempData["Error"] = "Unable to process order!";
                return RedirectToPage();
            }

            // Create the order
            bool orderCreated = Order.CreateOrder(email, productIds, totalWithFee, configuration);

            if (orderCreated)
            {
                TempData["Success"] = "Order placed successfully!";
                return RedirectToPage("/MyProfile");
            }
            else
            {
                TempData["Error"] = "Failed to create order. Please try again.";
                return RedirectToPage();
            }
        }

        public void DeleteItem(int ProductId)
        {
            Product.DeleteProduct(ProductId, HttpContext.Session.GetString("Email"), configuration);
            OnGet();
        }
        
        public void calculateSum()
        {
            foreach (var p in products)
            {
                PriceSum += Convert.ToDouble(p.Price);
            }
        }

        private string GetCartProductIds(string email)
        {
            string productIds = string.Empty;
            
            string sql = "SELECT products FROM Student WHERE Email = @Email";
            
            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        productIds = result.ToString();
                    }
                }
            }
            
            return productIds;
        }
    }
}
