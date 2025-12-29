using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using uni_swap.Models;

namespace uni_swap.Pages
{
    public class RegistrationModel : PageModel
    {
        
        public void OnGet()
        {
        }
        public void OnPost()
        {
            RegisterUser student = new RegisterUser(firstName: Request.Form["FirstName"], lastName: Request.Form["LastName"], email: Request.Form["Email"], password: Request.Form["Password"]);

            student.CreateUser();
            
        }
    }
}
