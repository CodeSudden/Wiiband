using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;  // Ensure this is the correct namespace for your ApplicationDbContext
using Capstone.Pages.Staff;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Capstone.Pages.Staff
{
    public class Staff_WiibandModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public Staff_WiibandModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string? CustomerName { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public int NumberOfJumpers { get; set; }
        [BindProperty]
        public int SelectedPromo { get; set; }
        [BindProperty]
        public bool DiscountPWD { get; set; }
        [BindProperty]
        public int TotalAmount { get; set; }
        [BindProperty]
        public string? SignatureData { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                ModelState.AddModelError(string.Empty, "Database connection string is missing.");
                return Page();
            }

            // Process the signature data (remove base64 prefix if it exists)
            string base64Signature = Regex.Replace(SignatureData ?? "", @"^data:image\/[a-z]+;base64,", string.Empty);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Customers (Customer_name, Customer_email, Num_jumpers, Promo, Discount, Total_amount, SignatureData) " +
                             "VALUES (@Customer_name, @Customer_email, @Num_jumpers, @Promo, @Discount, @Total_amount, @SignatureData)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Customer_name", CustomerName);
                    command.Parameters.AddWithValue("@Customer_email", Email);
                    command.Parameters.AddWithValue("@Num_jumpers", NumberOfJumpers);
                    command.Parameters.AddWithValue("@Promo", SelectedPromo);
                    command.Parameters.AddWithValue("@Discount", DiscountPWD);
                    command.Parameters.AddWithValue("@Total_amount", TotalAmount);
                    command.Parameters.AddWithValue("@SignatureData", base64Signature);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("dashboard");
        }

    }

}
