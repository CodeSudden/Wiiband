// Services/DashboardService.cs
using Capstone.Data;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone.Models
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DashboardDisplay> GetDashboardEntries(DateTime date)
        {
            // Fetch transactions for a given date (e.g., today)
            return _context.Transactions
                .Where(t => t.Date.Date == date)
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

        public int GetVisitorsCount()
        {
            return _context.GetTransactionCountFromVisitors();
        }

        public decimal GetTotalSalesToday()
        {
            return _context.GetTotalSalesForToday();
        }

        public int GetTotalJumpers()
        {
            return _context.GetTotalJumpers();
        }
    }
}
