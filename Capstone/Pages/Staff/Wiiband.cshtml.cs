using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Capstone.Hubs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Capstone.Data;  // Ensure this is the correct namespace for your ApplicationDbContext
using Capstone.Pages.Staff;

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

            TotalAmount = CalculateTotalAmount(NumberOfJumpers, SelectedPromo, DiscountPWD);

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

        private decimal CalculateTotalAmount(int numberOfJumpers, string selectedPromo, bool discountPWD)
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

            if (selectedPromo != "open" && discountPWD)
            {
                totalAmount -= promoRate * 0.2m;
            }

            return totalAmount;
        }
    }
}
