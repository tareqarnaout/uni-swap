using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using uni_swap.Models;

namespace uni_swap.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly IConfiguration configuration;
        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }

        public AdminPageModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult OnGet()
        {
            // Check if user is admin
            int? loggedIn = HttpContext.Session.GetInt32("Login");
            string userRole = HttpContext.Session.GetString("Role");

            if (loggedIn == null || loggedIn != 1 || userRole != "Admin")
            {
                return RedirectToPage("./Login");
            }

            Products = Product.GetProducts(configuration);
            Orders = GetAllOrders();
            return Page();
        }

        public IActionResult OnPostDeleteOrder(int orderId)
        {
            // Check if user is admin
            string userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Admin")
            {
                return Forbid();
            }

            DeleteOrder(orderId);
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteProduct(int productId)
        {
            // Check if user is admin
            string userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Admin")
            {
                return Forbid();
            }

            DeleteProduct(productId);
            return RedirectToPage();
        }

        private List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            
            string sql = @"SELECT o.OrderId, o.UserId, o.ProductIds, o.TotalAmount, o.OrderDate, o.Status
                          FROM Orders o
                          ORDER BY o.OrderDate DESC";

            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            OrderId = (int)reader["OrderId"],
                            UserId = (int)reader["UserId"],
                            ProductIds = reader["ProductIds"].ToString(),
                            TotalAmount = (decimal)reader["TotalAmount"],
                            OrderDate = (DateTime)reader["OrderDate"],
                            Status = reader["Status"].ToString()
                        };
                        orders.Add(order);
                    }
                }
            }

            // Load products for each order
            foreach (var order in orders)
            {
                order.Products = GetOrderProducts(order.ProductIds);
            }

            return orders;
        }

        private List<Product> GetOrderProducts(string productIds)
        {
            List<Product> products = new List<Product>();
            
            if (string.IsNullOrEmpty(productIds))
                return products;

            string sql = @"SELECT p.*
                          FROM Products p
                          WHERE p.ProductId IN (
                              SELECT CAST(value AS INT)
                              FROM STRING_SPLIT(@ProductIds, ',')
                              WHERE value IS NOT NULL AND value != ''
                          )";

            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductIds", productIds);
                    conn.Open();
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = (int)reader["ProductId"],
                            Title = reader["Title"].ToString(),
                            Price = reader["Price"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString(),
                            Description = reader["Description"].ToString(),
                            sellerId = (int)reader["SellerId"],
                            CreatedAt = reader["CreatedAt"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }

            return products;
        }

        private void DeleteOrder(int orderId)
        {
            string sql = "DELETE FROM Orders WHERE OrderId = @orderId";

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteProduct(int productId)
        {
            string sql = "DELETE FROM Products WHERE ProductId = @productId";

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
