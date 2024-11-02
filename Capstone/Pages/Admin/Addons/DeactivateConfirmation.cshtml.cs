using Capstone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace Capstone.Pages.Admin.Addons
{
    public class DeactivateConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeactivateConfirmationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string TransactionNumber { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(TransactionNumber))
            {
                return RedirectToPage("/Admin/Admin_Dashboard");
            }

            // Optionally, you could add a check here to verify if the transaction exists in the database.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(TransactionNumber))
            {
                return RedirectToPage("/Admin/Admin_Dashboard");
            }

            // Perform deactivation logic
            var transaction = await _context.Dashboard.FirstOrDefaultAsync(t => t.TransactionNumber == TransactionNumber);
            if (transaction != null)
            {
                // Assuming you're using a Status field to track deactivation
                transaction.Status = "Deactivated";  // or whatever status you want to set
                await _context.SaveChangesAsync();
            }

            // Redirect back to the dashboard after deactivation
            return RedirectToPage("/Admin/Admin_Dashboard");
        }
    }
}