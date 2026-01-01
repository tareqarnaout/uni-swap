using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using uni_swap.Models;

namespace uni_swap.Pages
{
    public class SellItemModel : PageModel
    {
        [BindProperty]
        public Item Item { get; set; } = new Item();

        public bool Success { get; set; } = false;

        public void OnGet()
        {
         
            if (HttpContext.Session.GetInt32("Login") != 1)
            {
                Response.Redirect("/Login");
            }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

         
            Item.SellerID = 1;

            Success = true;
        }
    }
}
