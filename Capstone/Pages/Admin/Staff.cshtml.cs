using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Capstone.Pages.Admin.Addons
{
    public class StaffModel : PageModel
    {
        public List<StaffMember> StaffMembers { get; set; }

        public void OnGet()
        {
            // Replace with your actual data fetching logic
            StaffMembers = new List<StaffMember>
            {
                new StaffMember { Id = 1, Name = "John Doe", AccessLevel = "Admin", Username = "lolol", ContactNumber = "123-456-7890", Shift = "9 AM - 5 PM" },
                new StaffMember { Id = 2, Name = "Jane Smith", AccessLevel = "Staff", Username = "roro",  ContactNumber = "987-654-3210", Shift = "10 AM - 6 PM" },
                // Add more staff members as needed
            };
        }
    }

    public class StaffMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string AccessLevel { get; set; }
        public string ContactNumber { get; set; }
        public string Shift { get; set; }
    }
}
