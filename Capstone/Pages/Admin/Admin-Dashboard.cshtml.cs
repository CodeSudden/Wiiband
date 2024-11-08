
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
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
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
            Transactions = _context.Transactions.ToList();
        }

        public Admin_DashboardModel(DashboardService dashboardService, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dashboardService = dashboardService;
        }
    }
}
