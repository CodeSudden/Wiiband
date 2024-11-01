namespace Capstone.Models
{
    public class CustomerData
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; }
        public string Wiiband { get; set; } // Unique ID for the wristband
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public string Status { get; set; }
        public int Battery { get; set; } // Battery level percentage
    }

}
