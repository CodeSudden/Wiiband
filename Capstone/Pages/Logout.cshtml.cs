using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Capstone.Pages
{
    // Logout.cshtml.cs
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear the session to log the user out
            HttpContext.Session.Clear();

            // Redirect to the login page
            return RedirectToPage("/Login");
        }
    }

}
