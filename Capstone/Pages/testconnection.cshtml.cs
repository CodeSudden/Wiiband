using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Pages
{
    public class TestconnectionModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private const int udpPort = 12345;  // Define the UDP port
        private const string udpIpAddress = "192.168.254.104";  // Define the UDP IP address

        public TestconnectionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? Message { get; set; }

        public void OnGet()
        {
            // Test the database connection
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
                }
            }
            catch (Exception ex)
            {
                Message = $"Database connection failed: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostAsync(string commandInput)
        {
            if (string.IsNullOrEmpty(commandInput))
            {
                // Return to the same page with an error message
                Message = "Command is empty or null.";
                return Page(); // Stay on the same page
            }

            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    var serverEndpoint = new IPEndPoint(IPAddress.Parse(udpIpAddress), udpPort);
                    byte[] messageBytes = Encoding.ASCII.GetBytes(commandInput);
                    await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndpoint);
                }

                // Return to the same page with a success message
                Message = $"Command sent successfully: {commandInput}";
                return Page(); // Stay on the same page
            }
            catch (Exception ex)
            {
                // Return to the same page with an error message
                Message = $"Error: {ex.Message}";
                return Page(); // Stay on the same page
            }
        }
    }
}
