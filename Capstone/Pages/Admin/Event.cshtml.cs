using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Capstone.Pages.Admin
{
    public class EventModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EventModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string? AddonsData { get; set; }
        public List<EventModelPopulate> UpcomingEvents { get; set; }
        public string? CalendarEventsJson { get; set; }
        public class EventModelPopulate
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
            public int Duration { get; set; }
            public int Jumpers { get; set; }
            public string? AddonsData { get; set; }
        }

        public void OnGet()
        {
            UpcomingEvents = new List<EventModelPopulate>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT Name, Email, Date, Time, Duration, Jumpers, Addons FROM Events";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = reader.GetDateTime(reader.GetOrdinal("Date"));
                            TimeSpan time = (TimeSpan)reader["Time"];
                            int duration = (int)reader["Duration"];
                            int jumpers = (int)reader["Jumpers"];

                            // Here, add logic to determine selected addons
                            List<string> Addons = new List<string>();

                            // Assuming you have some conditions to check if add-ons were selected
                            bool EInvitation = true; // Replace with actual condition
                            bool GameCoach = false; // Replace with actual condition
                            bool WaterBottle = true; // Replace with actual condition
                            bool MelonaIC = false; // Replace with actual condition

                            if (EInvitation) Addons.Add("E-Invitation");
                            if (GameCoach) Addons.Add("Game Coach");
                            if (WaterBottle) Addons.Add("Water Bottle");
                            if (MelonaIC) Addons.Add("Melona Ice Cream");

                            // Concatenate selected Addons into a single string before saving
                            string addonsData = Addons.Any() ? string.Join(", ", Addons) : "Null";

                            // Calculate start and end times
                            string starttime = time.ToString(@"hh\:mm");
                            string endtime = time.Add(TimeSpan.FromHours(duration)).ToString(@"hh\:mm");

                            UpcomingEvents.Add(new EventModelPopulate
                            {
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Date = date,
                                Time = time,
                                Duration = duration,
                                Jumpers = jumpers,
                                AddonsData = addonsData // Store the concatenated string
                            });
                        }
                    }
                }
            }

            // Transform UpcomingEvents to match FullCalendar's structure
            var calendarEvents = UpcomingEvents.Select(e => new
            {
                title = e.Name,
                start = e.Date.ToString("yyyy-MM-dd"), // Only the date part
                starttime = e.Time.ToString(@"hh\:mm"), // Format as HH:mm
                endtime = e.Time.Add(TimeSpan.FromHours(e.Duration)).ToString(@"hh\:mm"), // Calculate endtime
                extendedProps = new
                {
                    email = e.Email,
                    duration = e.Duration,
                    jumpers = e.Jumpers,
                    addons = e.AddonsData // Use the concatenated string
                }
            });

            CalendarEventsJson = JsonConvert.SerializeObject(calendarEvents);
        }
    }
}
