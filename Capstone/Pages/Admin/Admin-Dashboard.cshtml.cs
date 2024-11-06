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
        public List<Transactions> Transactions { get; set; } = new List<Transactions>();
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
            Transactions = _context.Transactions.ToList();
        }

        public Admin_DashboardModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
