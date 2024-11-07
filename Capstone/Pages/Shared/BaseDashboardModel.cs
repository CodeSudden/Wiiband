using Capstone.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Capstone.Pages.Shared
{
    public class BaseDashboardModel : PageModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public List<Transaction> Transactions { get; set; }
        public decimal TotalSales { get; protected set; }

        public BaseDashboardModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        protected void LoadDashboardData()
        {
            TotalSales = _context.GetTotalSales();
            Transactions = GetAllTransactions();
        }

        protected List<Transaction> GetAllTransactions()
        {
            return _context.Transactions.ToList();
        }
    }
}
