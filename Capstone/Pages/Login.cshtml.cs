using Capstone.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity; // Make sure to include this namespace
using System.Linq;

namespace Capstone.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<object> _passwordHasher;

        public LoginModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = new PasswordHasher<object>(); // Initialize the password hasher
        }

        [BindProperty]
        public required string Username { get; set; }

        [BindProperty]
        public required string Password { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == Username);

            if (user != null)
            {
                // Verify the password
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null, user.Password, Password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    // Password is correct; set session variables
                    if (_httpContextAccessor.HttpContext != null)
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("Type", user.Type);
                        _httpContextAccessor.HttpContext.Session.SetString("UserName", user.Username);
                        _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
                    }

                    // Redirect based on user type
                    if (user.Type == "Admin")
                    {
                        TempData["LoginSuccess"] = true;
                        return RedirectToPage("/Admin/Admin-Dashboard");
                    }
                    else if (user.Type == "Staff")
                    {
                        TempData["LoginSuccess"] = true;
                        return RedirectToPage("/Staff/Staff-Dashboard");
                    }
                }
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
