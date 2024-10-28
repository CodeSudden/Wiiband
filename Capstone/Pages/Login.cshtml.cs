using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Capstone.Pages // Ensure this namespace matches your project structure
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost() // This method must have a return type
        {
            // Staff credentials
            const string staffUsername = "staff";
            const string staffPassword = "staff123";

            // Admin credentials
            const string adminUsername = "admin";
            const string adminPassword = "admin123";

            // Authentication Logic
            if (Username == staffUsername && Password == staffPassword)
            {
                // Store the login success flag and redirect URL in TempData
                TempData["LoginSuccess"] = true;
                TempData["RedirectUrl"] = Url.Page("/Staff/Staff-Dashboard");

                return RedirectToPage(); // Correct return type
            }
            else if (Username == adminUsername && Password == adminPassword)
            {
                // Store the login success flag and redirect URL in TempData
                TempData["LoginSuccess"] = true;
                TempData["RedirectUrl"] = Url.Page("/Admin/Admin-Dashboard");

                return RedirectToPage(); // Correct return type
            }
            else
            {
                // If credentials are invalid, show an error message
                ErrorMessage = "Invalid username or password";
                return Page(); // Correct return type
            }
        }
    }
}
