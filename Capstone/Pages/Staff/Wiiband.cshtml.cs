using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Capstone.Data;
using Capstone.Models;
using System.Threading.Tasks;
using Capstone.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace Capstone.Pages.Staff
{
    public class WiibandModel : PageModel
    {
        private readonly IConfiguration _configuration;

        /* Constructor where both _context and _hubContext are injected
        public WiibandModel(ApplicationDbContext context, IHubContext<StaffDashboardHub> hubContext)
        {
            _configuration = configuration;
        }

        // Form properties
        [BindProperty, Required]
        public string CustomerName { get; set; }

        [BindProperty, Required, EmailAddress]
        public string Email { get; set; }

        [BindProperty, Required, Range(1, int.MaxValue, ErrorMessage = "Please enter at least 1 jumper.")]
        public int NumberOfJumpers { get; set; } = 1;

        [BindProperty, Required]
        public string SelectedPromo { get; set; }

        [BindProperty]
        public bool DiscountPWD { get; set; }
        public int TotalAmount { get; set; }
        public string? Waiver {  get; set; }

        [BindProperty, Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number of discounts.")]
        public int NumberOfDiscounts { get; set; } = 0;

        [BindProperty]
        public int WiibandID { get; set; }

        [BindProperty]
        public TimeSpan? StartTime { get; set; } = DateTime.Now.TimeOfDay;

        [BindProperty]
        public TimeSpan? EndTime { get; set; }

        [BindProperty, Required]
        public string Status { get; set; } = "Registered";

        [BindProperty]
        public string RemainingTime { get; set; }

        [BindProperty, Range(0, 100)]
        public decimal? BatteryLevel { get; set; } = 100;

        // Feedback message for UI
        [TempData]
        public string FeedbackMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // First, ensure validation is correct
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                FeedbackMessage = "There were errors with your submission. Please correct them and try again.";
                return Page();
            }

            try
            {
                // Generate the transaction number
                string transactionNumber = GenerateTransactionNumber();

                // Calculate total amount
                TotalAmount = CalculateTotalAmount(NumberOfJumpers, SelectedPromo, NumberOfDiscounts, DiscountPWD);

                // Create a new Dashboard object
                var dashboardEntry = new Dashboard
                {
                    TransactionNumber = transactionNumber,
                    WiibandID = WiibandID,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    RemainingTime = CalculateRemainingTime(StartTime, EndTime),
                    Status = Status,
                    BatteryLevel = BatteryLevel,
                    CustomerName = CustomerName,  // Save CustomerName
                    Email = Email                 // Save Email
                };

                // Save the entry in the database
                _context.Dashboard.Add(dashboardEntry);
                await _context.SaveChangesAsync();

                // Notify all clients via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveRegistrationUpdate", dashboardEntry);

                // Set success message and redirect to prevent form resubmission
                FeedbackMessage = "Registration successful! Your entry has been saved.";
                return RedirectToPage("/Staff/Staff_dashboard");
            }
            catch (Exception ex)
            {
                FeedbackMessage = $"An error occurred while saving your registration: {ex.Message}";
                Console.WriteLine($"Error: {ex.Message}");
                return Page();
            }
        }

        // Utility method to generate transaction number
        private string GenerateTransactionNumber()
        {
            return "TRN-" + DateTime.Now.Ticks;
        }

        // Utility method to calculate total amount based on promo and discount
        private decimal CalculateTotalAmount(int numberOfJumpers, string selectedPromo, int numberOfDiscounts, bool discountPWD)
        {
            var promoRates = new Dictionary<string, decimal>
            {
                { "499", 499 },
                { "399", 399 },
                { "3990", 3990 },
                { "7485", 7485 },
                { "open", 0 }
            };

            decimal promoRate = promoRates.ContainsKey(selectedPromo) ? promoRates[selectedPromo] : 0;
            decimal totalAmount = numberOfJumpers * promoRate;

            if (selectedPromo != "open")
            {
                int maxDiscounts = Math.Min(numberOfDiscounts, numberOfJumpers);
                decimal discountAmount = promoRate * 0.2m * maxDiscounts;
                totalAmount -= discountAmount;

                if (discountPWD)
                {
                    totalAmount -= promoRate * 0.2m; // Additional 20% discount for PWD
                }
            }

            return RedirectToPage("Success"); // Redirect to a success page
        }

        // Utility method to calculate remaining time between start and end time
        private string CalculateRemainingTime(TimeSpan? startTime, TimeSpan? endTime)
        {
            if (startTime.HasValue && endTime.HasValue)
            {
                TimeSpan remaining = endTime.Value - startTime.Value;
                return remaining.ToString(@"hh\:mm\:ss");
            }

            return "00:00:00";
        }

        // Log errors in the model state for debugging purposes
        private void LogModelErrors()
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Log errors for debugging
                }
            }
        }*/
    }

}
