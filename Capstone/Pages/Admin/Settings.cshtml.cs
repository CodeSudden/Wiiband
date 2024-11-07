using Capstone.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Capstone.Pages.Admin
{
    public class SettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingsModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty] public string? DisplayName { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public bool EmailNotifications { get; set; }
        [BindProperty] public bool SmsNotifications { get; set; }
        [BindProperty] public bool ProfileVisibility { get; set; }
        [BindProperty] public bool DataSharing { get; set; }
        [BindProperty] public string ThemeSelect { get; set; } = "light";
        [BindProperty] public string? Password { get; set; }
        [BindProperty] public string? ConfirmPassword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            var userType = _httpContextAccessor.HttpContext?.Session.GetString("Type");

            if (userId == null || userType != "Admin")
            {
                return RedirectToPage("/Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            // Populate the form fields with data from the database
            DisplayName = user.Display_name;
            Email = user.Email_address;
            EmailNotifications = user.Email_notify == true;
            SmsNotifications = user.Sms_notify == true;
            ProfileVisibility = user.Visibility == true;
            DataSharing = user.Third_party == true;
            ThemeSelect = user.Theme ?? "light";

            return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ModelState.AddModelError(string.Empty, "Both password fields are required.");
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<object>();
                user.Password = passwordHasher.HashPassword(null, Password);  // No need to pass 'null' here, as it's not required
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(); // Refresh the page to reflect changes or show a success message
        }

        public async Task<IActionResult> OnPostProfileSettingsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Display_name = DisplayName;
                user.Email_address = Email;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostNotificationSettingsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                // Convert bool to int for database storage (1 for true, 0 for false)
                user.Email_notify = EmailNotifications ? true : false;
                user.Sms_notify = SmsNotifications ? true : false;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPrivacySettingsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Visibility = ProfileVisibility ? true : false;  // Ensure Visibility is correctly converted to an int
                user.Third_party = DataSharing ? true : false;       // Ensure Third_party is correctly converted to an int
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostThemeSettingsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Theme = ThemeSelect;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
