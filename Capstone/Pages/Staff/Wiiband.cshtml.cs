using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;

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

        [BindProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number of discounts.")]
        public int NumberOfDiscounts { get; set; } = 0;

        public decimal TotalAmount { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a unique transaction number
            string transactionNumber = GenerateTransactionNumber();

            // Calculate the total amount with the number of discounts and other parameters
            TotalAmount = CalculateTotalAmount(NumberOfJumpers, SelectedPromo, NumberOfDiscounts, DiscountPWD);

            // Create a new Customer object with the registration data
            var customer = new Customer
            {
                CustomerName = CustomerName,
                Email = Email,
                NumberOfJumpers = NumberOfJumpers,
                DiscountPWD = DiscountPWD,
                TotalAmount = TotalAmount,
                TransactionNumber = transactionNumber,
            };

            // Notify the dashboard about the new registration using SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveRegistrationUpdate", new
            {
                CustomerName = CustomerName,
                NumberOfJumpers = NumberOfJumpers,
                TransactionNumber = transactionNumber
            });

            // Redirect to the dashboard after successful registration
            return RedirectToPage("/Staff/Staff_dashboard");
        }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Customers (Customer_name, Customer_email, Num_jumpers, Promo, Discount, Total_amount, Waiver) " +
                             "VALUES (@Customer_name, @Customer_email, @Num_jumpers, @Promo, @Discount, @Total_amount, @Waiver)";

        // Calculate the total amount considering discounts
        private decimal CalculateTotalAmount(int numberOfJumpers, string selectedPromo, int numberOfDiscounts, bool discountPWD)
        {
            var promoRates = new Dictionary<string, decimal>
            {
                { "499", 499 },
                { "399", 399 },
                { "3990", 3990 },
                { "7485", 7485 },
                { "open", 0 } // No charge for Open Time
            };

            decimal promoRate = promoRates.ContainsKey(selectedPromo) ? promoRates[selectedPromo] : 0;
            decimal totalAmount = numberOfJumpers * promoRate;

            // Apply discounts based on the number of jumpers and number of discounts
            if (selectedPromo != "open")
            {
                int maxDiscounts = Math.Min(numberOfDiscounts, numberOfJumpers); // Cap the discounts to the number of jumpers
                decimal discountAmount = promoRate * 0.2m * maxDiscounts;
                totalAmount -= discountAmount;

                if (discountPWD)
                {
                    totalAmount -= promoRate * 0.2m; // Apply PWD discount
                }
            }

            return RedirectToPage("Success"); // Redirect to a success page
        }
    }

}
