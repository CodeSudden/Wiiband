using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone.Pages.Staff
{
    public class Staff_dashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public List<DashboardDisplay> DashboardEntries { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }
        public int Visitors { get; private set; }
        public decimal TotalSalesToday { get; private set; }
        public int TotalJumpers { get; private set; } // New property for total jumpers

        public Staff_dashboardModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            var userName = _httpContextAccessor.HttpContext?.Session.GetString("UserName");
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            var userType = _httpContextAccessor.HttpContext?.Session.GetString("Type");

            if (userName == null || userId == null || userType != "Staff")
            {
                Response.Redirect("/Login");
                return;
            }

            TotalSalesToday = _context.GetTotalSalesForToday();

            // Update visitors count in the Visitors table
            _context.UpdateTransactionCountInVisitors();

            TotalJumpers = _context.GetTotalJumpers();

            // Set Visitors to the latest count from the Visitors table
            Visitors = _context.GetTransactionCountFromVisitors();

            // Filter transactions to include only those with today's date
            var today = DateTime.Today;
            DashboardEntries = _context.Transactions
                .Where(t => t.Date.Date == today) // Ensure the date is today
                .Select(t => new DashboardDisplay
                {
                    TransactionNumber = t.TransactionNumber,
                    WiibandID = t.WiibandID,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    RemainingTime = t.RemainingTime,
                    CustomerName = t.CustomerName,
                    Email = t.Email
                })
                .ToList();
        }

        public void OnPost()
        {
            var today = DateTime.Today;
            DashboardEntries = _context.Transactions
                .Where(t => (string.IsNullOrEmpty(StatusFilter) || t.Status == StatusFilter) && t.Date.Date == today) // Filter by date and status
                .Select(t => new DashboardDisplay
                {
                    TransactionNumber = t.TransactionNumber,
                    WiibandID = t.WiibandID,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    RemainingTime = t.RemainingTime,
                    CustomerName = t.CustomerName,
                    Email = t.Email
                })
                .ToList();
        }
    }
}
