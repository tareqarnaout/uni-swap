using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Http;
using uni_swap.Models;

namespace uni_swap.Pages
{
    public class LoginModel : PageModel
    {

        private readonly UserService userService;
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public LoginModel(UserService userService)
        {
            this.userService = userService;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {

            
            if (userService.ValidateUser(Email,Password))
            {
                if (userService.IsAdmin(Email))
                {
                    HttpContext.Session.SetInt32("Login", 1);
                    HttpContext.Session.SetString("Email", Email);
                    HttpContext.Session.SetString("Role", "Admin");
                    Response.Redirect("./AdminPage");
                }
                else
                {
                    HttpContext.Session.SetInt32("Login", 1);
                    HttpContext.Session.SetString("Email", Email);
                    HttpContext.Session.SetString("Role", "Student");
                    Response.Redirect("./HomePage");
                }
               
            }
            else
            {
                
                HttpContext.Session.SetInt32("Login", 0);
                HttpContext.Session.SetString("Email", "");
                Response.Redirect("./Login");
            }

        }
    }
}
