using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Capstone.Models;  // Assuming Dashboard is in the Models namespace

namespace Capstone.Hubs
{
    public class StaffDashboardHub : Hub
    {
        // Method to broadcast the Dashboard entry (or any object you want to send)
        public async Task SendRegistrationUpdate(Dashboard dashboardEntry)
        {
            await Clients.All.SendAsync("ReceiveRegistrationUpdate", dashboardEntry);
        }
    }
}
