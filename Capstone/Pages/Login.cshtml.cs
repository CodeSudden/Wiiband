using Capstone.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace Capstone.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public required string Username { get; set; }

        [BindProperty]
        public required string Password { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == this.Username && u.Password == this.Password);

            if (user != null)
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("Type", user.Type);
                    _httpContextAccessor.HttpContext.Session.SetString("UserName", user.Username);
                    _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
                }

                if (user.Type == "Admin")
                {
                    TempData["LoginSuccess"] = true;
                    return RedirectToPage("/Admin/Admin-Dashboard");
                }
                else if (user.Type.Equals("staff", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["LoginSuccess"] = true;
                    return RedirectToPage("/Staff/Staff-dashboard");
                }
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
