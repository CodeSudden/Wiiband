using Capstone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;


namespace Capstone.Pages.Admin
{
    public class Admin_DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<Transaction> Transactions { get; set; }
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }
        public decimal TotalSales { get; private set; }


        public void OnGet()
        {
            var Type = _httpContextAccessor.HttpContext?.Session.GetString("Type");
            var userName = _httpContextAccessor.HttpContext?.Session.GetString("UserName");
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

            if (userName == null || userId == null || Type == "admin")
            {
                Response.Redirect("/Login");
            }
            TotalSales = _context.GetTotalSales();

            // Simulate getting data from the database
            Transactions = GetAllTransactions();
        }

        public Admin_DashboardModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnPost()
        {
            // Simulate getting data from the database
            Transactions = GetAllTransactions();

            // Apply filtering logic
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                Transactions = Transactions.Where(t => t.Status.ToLower().Contains(StatusFilter.ToLower())).ToList();
            }
        }

        private List<Transaction> GetAllTransactions()
        {
            // Sample data
            return new List<Transaction>
            {
                new Transaction { TransactionNumber = "Trn-0011", Wiiband = "1", StartTime = "12:48pm", EndTime = "1:48pm", RemainingTime = "14 minutes", Status = "Online", Battery = "89%" },
                new Transaction { TransactionNumber = "Trn-0012", Wiiband = "2", StartTime = "12:51pm", EndTime = "1:51pm", RemainingTime = "24 minutes", Status = "Online", Battery = "90%" },
                new Transaction { TransactionNumber = "Trn-0013", Wiiband = "3", StartTime = "1:06pm", EndTime = "2:06pm", RemainingTime = "34 minutes", Status = "Online", Battery = "50%" },
                new Transaction { TransactionNumber = "--", Wiiband = "4", StartTime = "--", EndTime = "--", RemainingTime = "--", Status = "Offline", Battery = "63%" },
                new Transaction { TransactionNumber = "--", Wiiband = "5", StartTime = "--", EndTime = "--", RemainingTime = "--", Status = "Offline", Battery = "78%" }
            };
        }

        public class Transaction
        {
            public string TransactionNumber { get; set; }
            public string Wiiband { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string RemainingTime { get; set; }
            public string Status { get; set; }
            public string Battery { get; set; }
            public string Deactivate { get; set; }
        }
    }
}
