using Capstone.Data;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Pages.Staff
{
    public class Staff_dashboardModel : PageModel
    {
        private readonly DashboardService _dashboardService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const int udpPort = 12345; // Define the UDP port
        private const string udpIpAddress = "192.168.254.101"; // Define the UDP IP address

        public List<DashboardDisplay> DashboardEntries { get; set; } = new List<DashboardDisplay>();
        public List<WiibandMonitor> WiibandMonitors { get; set; } = new List<WiibandMonitor>();
        public string? Message { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }
        public int Visitors { get; private set; }
        public decimal TotalSalesToday { get; private set; }
        public int TotalJumpers { get; private set; }

        public Staff_dashboardModel(DashboardService dashboardService, ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dashboardService = dashboardService;
        }

        public void OnGet()
        {
            // Authentication and user session validation
            var userName = _httpContextAccessor.HttpContext?.Session.GetString("UserName");
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            var userType = _httpContextAccessor.HttpContext?.Session.GetString("Type");

            if (userName == null || userId == null || userType != "Staff")
            {
                Response.Redirect("/Login");
                return;
            }

            // Fetch general dashboard data
            var today = DateTime.Today;
            DashboardEntries = _dashboardService.GetDashboardEntries(today);
            TotalSalesToday = _dashboardService.GetTotalSalesToday();
            TotalJumpers = _dashboardService.GetTotalJumpers();
            Visitors = _dashboardService.GetVisitorsCount();

            // Fetch today's transaction data
            DashboardEntries = _context.Transactions
                .Where(t => t.Date.Date == today)
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

            // Fetch Wiiband monitor data from the database
            LoadWiibandMonitorData();
        }
        
        private void LoadWiibandMonitorData()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Message = "Connection string is missing!";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Message = "Database connection successful!";

                    // Load data from wiibandmonitor table
                    string sql = "SELECT wiibandtag, wiibandip, starttime, endtime FROM wiibandmonitor";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WiibandMonitors.Add(new WiibandMonitor
                                {
                                    WiibandTag = reader["wiibandtag"].ToString(),
                                    WiibandIP = reader["wiibandip"].ToString(),
                                    StartTime = Convert.ToDateTime(reader["starttime"]),
                                    EndTime = reader["endtime"] != DBNull.Value ? Convert.ToDateTime(reader["endtime"]) : (DateTime?)null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Database connection failed: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostSendCommandAsync(string commandInput)
        {
            if (string.IsNullOrEmpty(commandInput))
            {
                Message = "Command is empty or null.";
                return Page();
            }

            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    var serverEndpoint = new IPEndPoint(IPAddress.Parse(udpIpAddress), udpPort);
                    byte[] messageBytes = Encoding.ASCII.GetBytes(commandInput);
                    await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndpoint);
                }

                Message = $"Command sent successfully: {commandInput}";
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
            }

            return Page();
        }

        public class WiibandMonitor
        {
            public string WiibandTag { get; set; }
            public string WiibandIP { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
        }
    }
}
