using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Capstone.Pages.Staff
{
    public class StaffEventModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public StaffEventModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string? Name { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public int Phone { get; set; }
        [BindProperty]
        public DateOnly Date { get; set; }
        [BindProperty]
        public TimeOnly Time { get; set; }
        [BindProperty]
        public int Duration { get; set; }
        [BindProperty]
        public int Jumpers { get; set; }
        [BindProperty]
        public int Socks { get; set; }
        public string? Addons { get; set; }
        public int TrampolineGames { get; set; }
        public int PartyGuest { get; set; }
        public int PartyHours { get; set; }
        public int PartyDecorations { get; set; }
        public int ElecFoodCart { get; set; }
        public int PartyEquipCD { get; set; }
        public int PartyEquipUtils { get; set; }
        public DateTime Created_at { get; set; }

        public List<EventModel> UpcomingEvents { get; set; }
        public string? CalendarEventsJson { get; set; }

        public class EventModel
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
            public int Duration { get; set; }
            public int Jumpers { get; set; }
        }

        public void OnGet()
        {
            UpcomingEvents = new List<EventModel>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT Name, Email, Date, Time, Duration, Jumpers FROM Events";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UpcomingEvents.Add(new EventModel
                            {
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Time = (TimeSpan)reader["Time"],
                                Duration = (int)reader["Duration"],
                                Jumpers = (int)reader["Jumpers"]
                            });
                        }
                    }
                }
            }

            // Convert to JSON for FullCalendar
            CalendarEventsJson = JsonConvert.SerializeObject(UpcomingEvents);
        }

        public IActionResult OnPost()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                ModelState.AddModelError(string.Empty, "Database connection string is missing.");
                return Page();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Events 
                            (Name, Email, Phone, Date, Time, Duration, Jumpers, Socks, Addons, TrampolineGames,
                             PartyGuest, PartyHours, PartyDecorations, ElecFoodCart, PartyEquipCD, PartyEquipUtils, Created_at)
                             VALUES (@Name, @Email, @Phone, @Date, @Time, @Duration, @Jumpers, @Socks, @Addons, @TrampolineGames,
                             @PartyGuest, @PartyHours, @PartyDecorations, @ElecFoodCart, @PartyEquipCD, @PartyEquipUtils, @Created_at)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Date", Date == DateOnly.MinValue ? (object)DBNull.Value : Date);
                    command.Parameters.AddWithValue("@Time", Time == TimeOnly.MinValue ? (object)DBNull.Value : Time);
                    command.Parameters.AddWithValue("@Duration", Duration);
                    command.Parameters.AddWithValue("@Jumpers", Jumpers);
                    command.Parameters.AddWithValue("@Socks", Socks);
                    command.Parameters.AddWithValue("@Addons", Addons ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TrampolineGames", TrampolineGames);
                    command.Parameters.AddWithValue("@PartyGuest", PartyGuest);
                    command.Parameters.AddWithValue("@PartyHours", PartyHours);
                    command.Parameters.AddWithValue("@PartyDecorations", PartyDecorations);
                    command.Parameters.AddWithValue("@ElecFoodCart", ElecFoodCart);
                    command.Parameters.AddWithValue("@PartyEquipCD", PartyEquipCD);
                    command.Parameters.AddWithValue("@PartyEquipUtils", PartyEquipUtils);
                    command.Parameters.AddWithValue("@Created_at", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToPage("Success");
        }
    }
}
