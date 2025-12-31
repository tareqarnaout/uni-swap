using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using uni_swap.Models;

namespace uni_swap.Pages
{
    public class HomePageModel : PageModel
    {
        private readonly IConfiguration configuration;
        public List<Product> products;
        public HomePageModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult OnGet()
        {

            int? loggedIN = HttpContext.Session.GetInt32("Login"); 

            if (loggedIN == null || loggedIN != 1)
            {
                return RedirectToPage("./Login");
            }
            
            products = Product.GetProducts(configuration);
            return Page();
        }

        public IActionResult OnPost()
        {
            string id = Request.Form["productId"];
            AddtoCart(Convert.ToInt32(id));
            products = Product.GetProducts(configuration);
            return Page();
        }

        public void AddtoCart(int id)
        {
            Console.WriteLine(id);
            string sql = @"UPDATE Student 
                          SET products = CASE 
                                          WHEN products IS NULL OR products = '' THEN CAST(@ProductId AS VARCHAR(MAX))
                                          WHEN ',' + products + ',' LIKE '%,' + CAST(@ProductId AS VARCHAR(MAX)) + ',%' THEN products
                                          ELSE products + ',' + CAST(@ProductId AS VARCHAR(MAX))
                                        END
                          WHERE email = @Email";

            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("connectionString")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", id);
                        cmd.Parameters.AddWithValue("@Email", HttpContext.Session.GetString("Email"));
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");

            }

        }
    }
}
