using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone.Models
{
    public class Dashboard
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; }
        public int WiibandID { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string RemainingTime { get; set; }


        // New properties for customer details
        public string CustomerName { get; set; }
        public string Email { get; set; }
    }
}
