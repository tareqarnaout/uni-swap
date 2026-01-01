using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using uni_swap.Models;

namespace uni_swap.Pages
{
    public class MyProfileModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public MyProfileModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       
        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Email { get; set; }

     
        public IActionResult OnGet()
        {
          
            string email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
             
                return RedirectToPage("/Login");
            }

    
            string sql = "SELECT FirstName, LastName, Email FROM Student WHERE Email = @Email";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        FirstName = reader["FirstName"].ToString();
                        LastName = reader["LastName"].ToString();
                        Email = reader["Email"].ToString();
                    }
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                
                return RedirectToPage("/Login");
            }

            string sql = "UPDATE Student SET FirstName=@FirstName, LastName=@LastName WHERE Email=@Email";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Message"] = "Profile updated successfully!";
            return RedirectToPage("/MyProfile");
        }
    }
}
