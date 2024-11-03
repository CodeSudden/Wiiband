using Capstone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Capstone.Pages.Admin
{
    public class AnalyticsModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : PageModel
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public decimal TotalSales { get; private set; }
        public decimal DailySales { get; private set; }
        public decimal WeeklySales { get; private set; }
        public decimal MonthlySales { get; private set; }
        public decimal YearlySales { get; private set; }

        public void OnGet()
        {
            TotalSales = _context.GetTotalSales();
            DailySales = _context.GetTotalSalesForToday();
            WeeklySales = _context.GetWeeklySales();
            MonthlySales = _context.GetMonthlySales();
            YearlySales = _context.GetYearlySales();
        }
    }
}
