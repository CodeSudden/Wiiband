using Capstone.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Capstone.Pages.Admin.Addons
{
    public class StaffModel : PageModel
    {
        private readonly ApplicationDbContext _context; // Assuming you have this context

        public StaffModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class Staff
        {
            public int Id { get; set; }
            public string? Username { get; set; }
            public string? DisplayName { get; set; }
            public string? EmailAddress { get; set; }
            public int ContactNum { get; set; }
            public string? Shift { get; set; }
            public string? Type { get; set; } // Include this if you need to show access level
        }


        public List<Staff> StaffList { get; set; } = new List<Staff>(); // Property to hold staff data

        public void OnGet()
        {
            StaffList = _context.Users
                .Select(u => new Staff
                {
                    Id = u.Id,
                    Username = u.Username,
                    DisplayName = u.Display_name,
                    EmailAddress = u.Email_address,
                    ContactNum = u.ContactNum,
                    Shift = u.Shift,
                    Type = u.Type
                }).ToList();
        }
    }
}
