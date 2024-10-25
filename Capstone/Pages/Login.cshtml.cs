using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
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
            // Redirect to the staff dashboard
            return RedirectToPage("/Staff/Staff-Dashboard");
        }
        else if (Username == adminUsername && Password == adminPassword)
        {
            // Redirect to the admin dashboard
            return RedirectToPage("/Admin/Admin-Dashboard");
        }
        else
        {
            // If credentials are invalid, show an error message
            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
