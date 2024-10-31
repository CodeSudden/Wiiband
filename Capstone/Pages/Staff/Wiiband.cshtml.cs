using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;  // Ensure this is the correct namespace for your ApplicationDbContext
using Capstone.Pages.Staff;
using Microsoft.Data.SqlClient;

namespace Capstone.Pages.Staff
{
    public class WiibandModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public WiibandModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public int NumberOfJumpers { get; set; }
        public int SelectedPromo { get; set; }
        public bool DiscountPWD { get; set; }
        public int TotalAmount { get; set; }
        public string? Waiver {  get; set; }

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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Customers (Customer_name, Customer_email, Num_jumpers, Promo, Discount, Total_amount, Waiver) " +
                             "VALUES (@Customer_name, @Customer_email, @Num_jumpers, @Promo, @Discount, @Total_amount, @Waiver)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Customer_name", CustomerName);
                    command.Parameters.AddWithValue("@Customer_email", Email);
                    command.Parameters.AddWithValue("@Num_jumpers", NumberOfJumpers);
                    command.Parameters.AddWithValue("@Promo", SelectedPromo);
                    command.Parameters.AddWithValue("@Discount", DiscountPWD);
                    command.Parameters.AddWithValue("@Total_amount", TotalAmount);
                    command.Parameters.AddWithValue("@Waiver", Waiver ?? string.Empty);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("Success"); // Redirect to a success page
        }
    }

}
