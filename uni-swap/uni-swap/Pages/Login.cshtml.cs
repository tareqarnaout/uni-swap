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

        private readonly UserService _userService;
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {

            bool loggedIn = false;
            if (_userService.ValidateUser(Email,Password))
            {
                loggedIn = true;
                HttpContext.Session.SetInt32("Login", 1);
                HttpContext.Session.SetString("Email", Email);
                Response.Redirect("./Home");
            }
            else
            {
                loggedIn = false;
                HttpContext.Session.SetInt32("Login", 0);
                HttpContext.Session.SetString("Email", "");
                Response.Redirect("./User");
            }

        }
    }
}
