using Capstone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Pages.Admin.Addons
{
    public class DeactivateConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int udpPort = 12345;  // Define the UDP port
        private const string udpIpAddress = "192.168.254.104";  // Define the UDP IP address

        public DeactivateConfirmationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string TransactionNumber { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(TransactionNumber))
            {
                return RedirectToPage("/Admin/Admin_Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(TransactionNumber))
            {
                return RedirectToPage("/Admin/Admin_Dashboard");
            }

            // Send multiple UDP commands for deactivation confirmation
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    var serverEndpoint = new IPEndPoint(IPAddress.Parse(udpIpAddress), udpPort);

                    // List of commands to send
                    string[] commands = new string[]
                    {
                        "17 E7 29 66,VIBRATE",   // Vibrate
                        "17 E7 29 66,BUZZER,500,10",  // Buzzer
                        "17 E7 29 66,REDON",     // Red Light On
                        "17 E7 29 66,ALLOFF"     // All Off
                    };

                    // Send each command
                    foreach (string command in commands)
                    {
                        byte[] messageBytes = Encoding.ASCII.GetBytes(command);
                        await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndpoint);
                        await Task.Delay(500); // Short delay between commands, if needed
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally, log or handle exceptions if needed
                ModelState.AddModelError(string.Empty, $"Error sending UDP commands: {ex.Message}");
            }

            // Redirect back to the dashboard after deactivation
            return RedirectToPage("/Admin/Admin_Dashboard");
        }
    }
}
