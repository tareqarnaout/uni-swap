using Microsoft.Data.SqlClient;

namespace uni_swap.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ProductIds { get; set; } // Comma-separated product IDs
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // e.g., "Completed", "Pending", "Cancelled"

        public List<Product> Products { get; set; } = new List<Product>();

        public static bool CreateOrder(string email, string productIds, decimal totalAmount, IConfiguration configuration)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
                {
                    // First, get the UserId from email
                    string getUserIdSql = "SELECT UserID FROM Student WHERE Email = @Email";
                    int userId;

                    using (SqlCommand cmd = new SqlCommand(getUserIdSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        conn.Open();
                        var result = cmd.ExecuteScalar();
                        if (result == null)
                            return false;
                        
                        userId = (int)result;
                    }

                    // Insert the order
                    string insertOrderSql = @"INSERT INTO Orders (UserId, ProductIds, TotalAmount, Status) 
                                             VALUES (@UserId, @ProductIds, @TotalAmount, 'Completed')";

                    using (SqlCommand cmd = new SqlCommand(insertOrderSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@ProductIds", productIds);
                        cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        
                        cmd.ExecuteNonQuery();
                    }

                    // Clear the user's cart after successful order
                    string clearCartSql = "UPDATE Student SET products = NULL WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(clearCartSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                return false;
            }
        }

        public static List<Order> GetUserOrders(string email, IConfiguration configuration)
        {
            List<Order> orders = new List<Order>();
            
            string sql = @"SELECT o.OrderId, o.UserId, o.ProductIds, o.TotalAmount, o.OrderDate, o.Status
                          FROM Orders o
                          INNER JOIN Student s ON o.UserId = s.UserID
                          WHERE s.Email = @Email
                          ORDER BY o.OrderDate DESC";

            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
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
                order.Products = GetOrderProducts(order.ProductIds, configuration);
            }

            return orders;
        }

        private static List<Product> GetOrderProducts(string productIds, IConfiguration configuration)
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
    }
}