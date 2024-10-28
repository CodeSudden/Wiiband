using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Capstone.Hubs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;

namespace Capstone.Pages.Staff
{
    public class WiibandModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<StaffDashboardHub> _hubContext;

        public WiibandModel(ApplicationDbContext context, IHubContext<StaffDashboardHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [BindProperty]
        [Required]
        public string CustomerName { get; set; }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter at least 1 jumper.")]
        public int NumberOfJumpers { get; set; } = 1;

        [BindProperty]
        [Required]
        public string SelectedPromo { get; set; }

        [BindProperty]
        public bool DiscountPWD { get; set; }

        [BindProperty]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number of discounts.")]
        public int NumberOfDiscounts { get; set; } = 0;

        public decimal TotalAmount { get; set; }

        // Automatically generate the transaction number in the server-side code
        public async Task<IActionResult> OnPostAsync()
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

        // Generate a unique transaction number
        private string GenerateTransactionNumber()
        {
            return "TRN-" + DateTime.Now.Ticks.ToString(); // Example: TRN-637123456789012345
        }

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

            return totalAmount;
        }
    }
}
