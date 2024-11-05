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
                connection.Open();

                // Start a SQL transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Insert Customer record
                        string customerSql = @"
                    INSERT INTO Customers (Customer_name, Customer_email, Num_jumpers, Promo, Discount, Total_amount, SignatureData, Created_at) 
                    VALUES (@Customer_name, @Customer_email, @Num_jumpers, @Promo, @Discount, @Total_amount, @SignatureData, @Created_at);
                    SELECT SCOPE_IDENTITY();";

                        SqlCommand customerCommand = new SqlCommand(customerSql, connection, transaction);
                        customerCommand.Parameters.AddWithValue("@Customer_name", CustomerName);
                        customerCommand.Parameters.AddWithValue("@Customer_email", Email);
                        customerCommand.Parameters.AddWithValue("@Num_jumpers", NumberOfJumpers);
                        customerCommand.Parameters.AddWithValue("@Promo", SelectedPromo);
                        customerCommand.Parameters.AddWithValue("@Discount", DiscountPWD);
                        customerCommand.Parameters.AddWithValue("@Total_amount", TotalAmount);
                        customerCommand.Parameters.AddWithValue("@SignatureData", base64Signature);
                        DateTime datenow = DateTime.Now;
                        customerCommand.Parameters.AddWithValue("@Created_at", datenow);

                        // Execute customer insertion and get the new Customer ID
                        int customerId = Convert.ToInt32(customerCommand.ExecuteScalar());

                        // Step 2: Generate the next Transaction Number
                        string transactionNumber;
                        string transactionNumberSql = "SELECT COUNT(*) + 1 FROM Transactions";
                        SqlCommand transactionNumberCommand = new SqlCommand(transactionNumberSql, connection, transaction);
                        int nextTransactionNumber = (int)transactionNumberCommand.ExecuteScalar();
                        transactionNumber = $"TRN-{nextTransactionNumber:D3}";

                        // Step 3: Insert Transaction record
                        string transactionSql = @"
                    INSERT INTO Transactions (TransactionNumber, WiibandID, CustomerName, Email, Amount, Date, Status) 
                    VALUES (@TransactionNumber, @WiibandID, @CustomerName, @Email, @Amount, @Date, @Status)";

                        SqlCommand transactionCommand = new SqlCommand(transactionSql, connection, transaction);
                        transactionCommand.Parameters.AddWithValue("@TransactionNumber", transactionNumber);
                        transactionCommand.Parameters.AddWithValue("@WiibandID", customerId); // linking to the Customer ID
                        transactionCommand.Parameters.AddWithValue("@CustomerName", CustomerName);
                        transactionCommand.Parameters.AddWithValue("@Email", Email);
                        transactionCommand.Parameters.AddWithValue("@Amount", TotalAmount);
                        transactionCommand.Parameters.AddWithValue("@Date", datenow);
                        transactionCommand.Parameters.AddWithValue("@Status", "Offline");

                        transactionCommand.ExecuteNonQuery();

                        // Commit transaction
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Rollback transaction if any error occurs
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, "An error occurred while processing the transaction.");
                        return Page();
                    }
                }
            }

            return RedirectToPage("Staff-Dashboard");
        }



    }

}
