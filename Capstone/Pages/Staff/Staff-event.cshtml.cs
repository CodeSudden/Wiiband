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
        [BindProperty]
        public bool EInvitation { get; set; }
        [BindProperty]
        public bool GameCoach { get; set; }
        [BindProperty]
        public bool WaterBottle { get; set; }
        [BindProperty]
        public bool MelonaIC { get; set; }
        [BindProperty]
        public List<string> Addons { get; set; } = new();
        [BindProperty]
        public int TrampolineGames { get; set; }
        [BindProperty]
        public int PartyGuest { get; set; }
        [BindProperty]
        public int PartyHours { get; set; }
        [BindProperty]
        public string? PartyDecorations { get; set; }
        [BindProperty]
        public int ElecFoodCart { get; set; }
        [BindProperty]
        public int PartyEquipCD { get; set; }
        [BindProperty]
        public int PartyEquipUtils { get; set; }
        [BindProperty]
        public DateTime Created_at { get; set; }
        [BindProperty]
        public string? AddonsData { get; set; }
        [BindProperty]
        public List<EventModel> UpcomingEvents { get; set; }
        [BindProperty]
        public string? CalendarEventsJson { get; set; }
        public class EventModel
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
            UpcomingEvents = new List<EventModel>();
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
                            bool EInvitation = true;
                            bool GameCoach = false; 
                            bool WaterBottle = true;
                            bool MelonaIC = false; 

                            if (EInvitation) Addons.Add("E-Invitation");
                            if (GameCoach) Addons.Add("Game Coach");
                            if (WaterBottle) Addons.Add("Water Bottle");
                            if (MelonaIC) Addons.Add("Melona Ice Cream");

                            // Concatenate selected Addons into a single string before saving
                            string addonsData = Addons.Any() ? string.Join(", ", Addons) : "Null";

                            // Calculate start and end times
                            string starttime = time.ToString(@"hh\:mm");
                            string endtime = time.Add(TimeSpan.FromHours(duration)).ToString(@"hh\:mm");

                            UpcomingEvents.Add(new EventModel
                            {
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Date = date,
                                Time = time,
                                Duration = duration,
                                Jumpers = jumpers,
                                AddonsData = addonsData
                            });
                        }
                    }
                }
            }

            // Transform UpcomingEvents to match FullCalendar's structure
            var calendarEvents = UpcomingEvents.Select(e => new
            {
                title = e.Name,
                start = e.Date.ToString("yyyy-MM-dd"),
                starttime = e.Time.ToString(@"hh\:mm"), 
                endtime = e.Time.Add(TimeSpan.FromHours(e.Duration)).ToString(@"hh\:mm"),
                extendedProps = new
                {
                    email = e.Email,
                    duration = e.Duration,
                    jumpers = e.Jumpers,
                    addons = e.AddonsData
                }
            });

            CalendarEventsJson = JsonConvert.SerializeObject(calendarEvents);
        }

        public string SuggestPackage()
        {
            int baseCost = 499 * Jumpers;  // Base cost calculation based on number of jumpers
            int totalCost = baseCost;
            var suggestedAddons = new List<string>();

            // 1. Duration-Based Recommendations
            if (Duration == 1)
            {
                suggestedAddons.Add("Standard Package: 1 Hour Jump");
            }
            else if (Duration == 2)
            {
                suggestedAddons.Add("Extended Package: 2 Hours Jump");
                totalCost += 200;  // Additional cost for extended time
            }
            else if (Duration >= 3)
            {
                suggestedAddons.Add("Premium Package: 3+ Hours Jump");
                totalCost += 400;  // Additional cost for premium time
            }

            // 2. Add Recommendations Based on Number of Jumpers
            if (Jumpers > 10)
            {
                suggestedAddons.Add("Group Discount");
                totalCost -= 300;  // Apply a discount for larger groups
            }
            if (Socks > 0)
            {
                suggestedAddons.Add($"Socks: {Socks} pairs");
                totalCost += Socks * 50;  // Cost per pair of socks
            }

            // 3. Add-on Recommendations Based on User Selection
            if (EInvitation) { suggestedAddons.Add("E-Invitation"); totalCost += 100; }
            if (GameCoach) { suggestedAddons.Add("Game Coach"); totalCost += 300; }
            if (WaterBottle) { suggestedAddons.Add("Water Bottle Package"); totalCost += Jumpers * 20; }
            if (MelonaIC) { suggestedAddons.Add("Melona Ice Cream"); totalCost += Jumpers * 30; }

            // 4. Party Area and Decorations
            if (!string.IsNullOrEmpty(PartyDecorations))
            {
                switch (PartyDecorations)
                {
                    case "HalfDeck":
                        suggestedAddons.Add("Half Deck Decorations");
                        totalCost += 8000;
                        break;
                    case "WholeDeck":
                        suggestedAddons.Add("Whole Deck Decorations");
                        totalCost += 15000;
                        break;
                    case "Premium":
                        suggestedAddons.Add("Premium Decorations");
                        totalCost += 25000;
                        break;
                }
            }

            // Return suggested package and estimated cost
            return $"Estimated Cost: ₱{totalCost}. Recommended Package Add-ons: {string.Join(", ", suggestedAddons)}";
        }


        public IActionResult OnPostSuggestPackage([FromBody] StaffEventModel model)
        {
            var suggestion = SuggestPackage();
            return new JsonResult(new { suggestion = suggestion });
        }

        public IActionResult OnPost()
        {
            if (EInvitation) Addons.Add("E-Invitation");
            if (GameCoach) Addons.Add("Game Coach");
            if (WaterBottle) Addons.Add("Water Bottle");
            if (MelonaIC) Addons.Add("Melona Ice Cream");
            // Concatenate selected Addons into a single string before saving
            if (Addons != null && Addons.Any())
            {
                AddonsData = string.Join(", ", Addons); // Join list into a single string
            }
            else
            {
                AddonsData = "Null";
            }
            if (!Request.Form.ContainsKey("partyDecorations"))
            {
                PartyDecorations = null;
            }
            if (!Request.Form.ContainsKey("ElecFoodCart"))
            {
                ElecFoodCart = 0;
            }

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
                    command.Parameters.AddWithValue("@Name", Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Date", Date == DateOnly.MinValue ? (object)DBNull.Value : Date);
                    command.Parameters.AddWithValue("@Time", Time == TimeOnly.MinValue ? (object)DBNull.Value : Time);
                    command.Parameters.AddWithValue("@Duration", Duration);
                    command.Parameters.AddWithValue("@Jumpers", Jumpers);
                    command.Parameters.AddWithValue("@Socks", Socks);
                    command.Parameters.AddWithValue("@Addons", AddonsData ?? (object)DBNull.Value);
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

            return RedirectToPage("/Staff/Staff-event");
        }
    }
}
