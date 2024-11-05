using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Capstone.Pages.Staff
{
    public class Staff_dashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public List<Transactions> Transactions { get; set; } = new List<Transactions>();
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }
        public decimal TotalSalesToday { get; private set; }

        public void OnGet()
        {
            var Type = _httpContextAccessor.HttpContext?.Session.GetString("Type");
            var userName = _httpContextAccessor.HttpContext?.Session.GetString("UserName");
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");

            if (userName == null || userId == null || Type == "staff")
            {
                Response.Redirect("/Login");
            }

            TotalSalesToday = _context.GetTotalSalesForToday();
            Transactions = _context.Transactions.ToList();

        }

        public Staff_dashboardModel(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


    }
}
