using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Capstone.Pages.Admin.Addons
{
    public class AddStaffModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string AccessLevel { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ContactNumber { get; set; }

        [BindProperty]
        public string Shift { get; set; }

        public void OnGet()
        {
            // Initialization logic if needed
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Logic to save the new staff member to the database would go here
            // For example:
            // _context.StaffMembers.Add(new StaffMember { Name = Name, AccessLevel = AccessLevel, ContactNumber = ContactNumber, Shift = Shift });
            // _context.SaveChanges();

            // Redirect to the Staff page after creation
            return RedirectToPage("/Admin/Staff");
        }
    }
}
