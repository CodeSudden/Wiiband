using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Capstone.Pages.Admin.Addons
{
    public class AddStaffModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AddStaffModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public required string Password { get; set; }
        [BindProperty]
        public string? Type { get; set; }
        [BindProperty]
        public string? Display_name { get; set; }
        [BindProperty]
        public string? Email_address { get; set; }
        [BindProperty]
        public int ContactNum { get; set; } 
        [BindProperty]
        public string? Shift { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Database connection string
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                ModelState.AddModelError(string.Empty, "Database connection string is missing.");
                return Page();
            }

            var passwordHasher = new PasswordHasher<object>(); // Use an instance of PasswordHasher
            string hashedPassword = passwordHasher.HashPassword(null, Password); // Hash the password

            // SQL Insert Command
            string insertCommand = @"
                INSERT INTO Users (Username, Password, Type, Display_name, Email_address, Email_notify, Sms_notify, Visibility, Third_party, Theme, Created_at, Updated_at, ContactNum, Shift)
                VALUES (@Username, @Password, @Type, @Display_name, @Email_address, @Email_notify, @Sms_notify, @Visibility, @Third_party, @Theme, @Created_at, @Updated_at, @ContactNum, @Shift)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(insertCommand, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", hashedPassword ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Type", Type ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Display_name", Display_name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email_address", Email_address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email_notify", false); // Default values for optional fields
                    command.Parameters.AddWithValue("@Sms_notify", false);
                    command.Parameters.AddWithValue("@Visibility", false);
                    command.Parameters.AddWithValue("@Third_party", false);
                    command.Parameters.AddWithValue("@Theme", "light");
                    command.Parameters.AddWithValue("@Created_at", DateTime.Now);
                    command.Parameters.AddWithValue("@Updated_at", DateTime.Now);
                    command.Parameters.AddWithValue("@ContactNum", ContactNum);
                    command.Parameters.AddWithValue("@Shift", Shift ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToPage("../Staff"); // Redirect to a success page after insertion
        }
    }
}
