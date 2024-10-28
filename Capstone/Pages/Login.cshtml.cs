using Capstone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace Capstone.Pages // Replace with your actual namespace
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                if (user.Type == "Admin")
                {
                    return RedirectToPage("/Admin/Admin-Dashboard");
                }
                else if (user.Type == "Staff")
                {
                    return RedirectToPage("/Staff/Staff-Dashboard");
                }
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
