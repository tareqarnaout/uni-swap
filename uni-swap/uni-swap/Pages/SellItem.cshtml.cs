using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace uni_swap.Pages
{
    public class SellItemModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public SellItemModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        [BindProperty]
        [Required(ErrorMessage = "Item name is required")]
        public string Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Price is required")]
        [Range(0.1, 10000, ErrorMessage = "Enter a valid price")]
        public string Price { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please upload an image")]
        public IFormFile ImageFile { get; set; }


        [BindProperty]
        public string Description { get; set; }

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Login");

            string imageUrl = await SaveFileAsync(ImageFile, "images");
            

            using SqlConnection conn = new SqlConnection(
                _configuration.GetConnectionString("ConnectionString"));

            string sql = @"
                INSERT INTO Products (Title, Price, ImageUrl, Description, sellerId, CreatedAt)
                VALUES (
                    @Title,
                    @Price,
                    @ImageUrl,
                    @Description,
                    (SELECT UserID FROM Student WHERE Email=@Email),
                    @CreatedAt
                )";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@Price", Price);
            cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);
            cmd.Parameters.AddWithValue("@Description", Description ?? "");
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            await cmd.ExecuteNonQueryAsync();

            TempData["Message"] = "Item published successfully!";
            return RedirectToPage();
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return "";

            // Create unique filename
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Create the upload directory path
            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", folderName);

            // Ensure directory exists
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string filePath = Path.Combine(uploadFolder, fileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path for database storage
            return $"/uploads/{folderName}/{fileName}";
        }
    }
}