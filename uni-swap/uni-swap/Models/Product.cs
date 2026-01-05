using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Http;

namespace uni_swap.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public String Price{ get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int sellerId { get; set;}
        public string sellerName { get; set; }
        public string CreatedAt {  get; set; }


        static public List<Product> GetProducts(IConfiguration _configuration)
        {
            List<Product> products = new List<Product>();
            string sql = "select * from Products";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("connectionString")))
            {

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if(dr != null)
                    {
                        while(dr.Read())
                        {
                            Product temp = new Product();
                            temp.Id = (int)dr["ProductId"];
                            temp.Title = dr["Title"].ToString();
                            temp.Price = dr["Price"].ToString();
                            temp.ImageUrl = dr["ImageUrl"].ToString();
                            temp.sellerId = (int)dr["sellerId"];
                            temp.CreatedAt = dr["CreatedAt"].ToString();
                            temp.Description = dr["Description"].ToString();

                            string UserSql = "select firstname, lastName from student where userid = @userid ";
                           
                            using (SqlConnection getUserName = new SqlConnection(_configuration.GetConnectionString("connectionString")))
                            {
                                using (SqlCommand getUserNameCmd = new SqlCommand(UserSql, getUserName))
                                {
                                    getUserNameCmd.Parameters.AddWithValue("@userid", temp.sellerId);
                                    getUserName.Open();
                                    SqlDataReader name = getUserNameCmd.ExecuteReader();
                                    while (name.Read())
                                    {
                                        temp.sellerName = name["FirstName"].ToString() + " "+ name["LastName"].ToString();
                                    }
                                }
                            }
                            products.Add(temp);
                         
                        }
                    }
                }
            }
            return products;
        }

        static public List<Product> getCart(string email, IConfiguration _configuration)
        {
            List<Product> products = new List<Product>();
            string sql = @"SELECT p.*
                          FROM Products p
                          WHERE p.ProductId IN (
                              SELECT CAST(value AS INT)
                              FROM STRING_SPLIT((SELECT products FROM Student WHERE email = @Email), ',')
                              WHERE value IS NOT NULL AND value != ''
                          )
                          ORDER BY p.ProductId";
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("connectionString")))
            {

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                        
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            Product temp = new Product();
                            temp.Id = (int)dr["ProductId"];
                            temp.Title = dr["Title"].ToString();
                            temp.Price = dr["Price"].ToString();
                            temp.ImageUrl = dr["ImageUrl"].ToString();
                            temp.sellerId = (int)dr["sellerId"];
                            temp.CreatedAt = dr["CreatedAt"].ToString();
                            temp.Description = dr["Description"].ToString();
                            products.Add(temp);

                        }
                    }
                }
            }
            return products;
        }
        static public void DeleteProduct(int ProductId, string email, IConfiguration _configuration)
        {
            string sql = @"UPDATE Student 
                          SET products = CASE 
                                          WHEN products = CAST(@ProductId AS VARCHAR(MAX)) THEN NULL
                                          WHEN products LIKE CAST(@ProductId AS VARCHAR(MAX)) + ',%' THEN SUBSTRING(products, LEN(@ProductId) + 2, LEN(products))
                                          WHEN products LIKE '%,' + CAST(@ProductId AS VARCHAR(MAX)) THEN LEFT(products, LEN(products) - LEN(@ProductId) - 1)
                                          WHEN products LIKE '%,' + CAST(@ProductId AS VARCHAR(MAX)) + ',%' THEN REPLACE(products, ',' + CAST(@ProductId AS VARCHAR(MAX)) + ',', ',')
                                          ELSE products
                                        END
                          WHERE email = @Email AND products IS NOT NULL";
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("connectionString")))
            {

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ProductId", ProductId);
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    
                }
            }
        }
    }
}
