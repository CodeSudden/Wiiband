using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Capstone.Hubs
{
    public class StaffDashboardHub : Hub
    {
        // Method to receive registration completion and broadcast to all clients
        public async Task CompleteRegistration(RegistrationData data)
        {
            // Send this data to all connected clients (the dashboard)
            await Clients.All.SendAsync("ReceiveRegistrationUpdate", data);
        }
    }

    // Helper class to store registration data
    public class RegistrationData
    {
        public string CustomerName { get; set; }
        public int NumberOfJumpers { get; set; }
        public string TotalAmount { get; set; }
    }
}
