
using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using static Capstone.Data.ApplicationDbContext;
using static Capstone.Pages.Staff.Staff_dashboardModel;


namespace Capstone.Pages.Admin
{
    public class Admin_DashboardModel : PageModel
    {
        private readonly DashboardService _dashboardService;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public List<Transaction> Transactions { get; set; }
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }
        public decimal TotalSales { get; private set; }
        public int Visitors { get; private set; }
        public decimal TotalSalesToday { get; private set; }
        public int TotalJumpers { get; private set; }

        private const int udpPort = 12345; // Define the UDP port
        private const string udpIpAddress = "192.168.254.101"; // Define the UDP IP address
        public List<DashboardDisplay> DashboardEntries { get; set; } = new List<DashboardDisplay>();
        public List<WiibandMonitor> WiibandMonitors { get; set; } = new List<WiibandMonitor>();


        public void OnGet()
        {
            var Type = _httpContextAccessor.HttpContext?.Session.GetString("Type");
            var userName = _httpContextAccessor.HttpContext?.Session.GetString("UserName");
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

            if (userName == null || userId == null || Type == "admin")
            {
                Response.Redirect("/Login");
            }
            var today = DateTime.Today;
            DashboardEntries = _dashboardService.GetDashboardEntries(today);
            TotalSalesToday = _dashboardService.GetTotalSalesToday();
            TotalJumpers = _dashboardService.GetTotalJumpers();
            Visitors = _dashboardService.GetVisitorsCount();

            // Simulate getting data from the database
            Transactions = GetAllTransactions();
        }

        public Admin_DashboardModel(DashboardService dashboardService, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dashboardService = dashboardService;
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
                new Transaction { TransactionNumber = "Trn-0011", Wiiband = "1", CustomerName = "Tine", Email = "tine@gmail.com",StartTime = "12:48pm", EndTime = "1:48pm", RemainingTime = "14 minutes"},
                new Transaction { TransactionNumber = "Trn-0012", Wiiband = "2",CustomerName = "Rain", Email = "Rain@gmail.com", StartTime = "12:51pm", EndTime = "1:51pm", RemainingTime = "24 minutes" },
                new Transaction { TransactionNumber = "Trn-0013", Wiiband = "3", CustomerName = "Khalin", Email = "Khalin@gmail.com",StartTime = "1:06pm", EndTime = "2:06pm", RemainingTime = "34 minutes" },
                new Transaction { TransactionNumber = "--", Wiiband = "4", StartTime = "--", EndTime = "--", RemainingTime = "--", Status = "Offline", Battery = "63%" },
                new Transaction { TransactionNumber = "--", Wiiband = "5", StartTime = "--", EndTime = "--", RemainingTime = "--", Status = "Offline", Battery = "78%" }
            };
        }

        public class Transaction
        {
            public string TransactionNumber { get; set; }
            public string Wiiband { get; set; }
            public string CustomerName { get; set; }
            public string Email { get; set; }

            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string RemainingTime { get; set; }
            public string Status { get; set; }
            public string Battery { get; set; }
            public string Deactivate { get; set; }
        }
    }
}
