using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace uni_swap.Pages
{
    public class SellItemModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public SellItemModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        [Required(ErrorMessage = "Item name is required")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Price is required")]
        [Range(0.1, 10000, ErrorMessage = "Enter a valid price")]
        public decimal Price { get; set; }

        [BindProperty]
        public string ImageUrl { get; set; }

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            using SqlConnection conn = new SqlConnection(
                _configuration.GetConnectionString("ConnectionString"));

            string sql = @"
                INSERT INTO Item (Name, Price, ImageUrl, SellerID)
                VALUES (
                    @Name,
                    @Price,
                    @ImageUrl,
                    (SELECT StudentID FROM Student WHERE Email=@Email)
                )";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Price", Price);
            cmd.Parameters.AddWithValue("@ImageUrl", ImageUrl ?? "");
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            cmd.ExecuteNonQuery();

            TempData["Message"] = "Item published successfully!";
            return RedirectToPage();
        }
    }
}

