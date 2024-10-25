using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Capstone.Pages.Staff
{
    public class StaffEventModel : PageModel
    {
        public List<EventModel> UpcomingEvents { get; set; }

        public string CalendarEventsJson { get; set; }

        public void OnGet()
        {
            // Initialize the upcoming events
            UpcomingEvents = new List<EventModel>
            {
                 new EventModel
                {
                    Name = "birthday",
                    Date = new DateTime(2024, 10, 17),
                    Location = "Fiesta carnival",
                    Attendees = 12,
                    IsConfirmed = true
                },
                new EventModel
                {
                    Name = "Anica's birthday",
                    Date = new DateTime(2024, 10, 22),
                    Location = "Fiesta carnival",
                    Attendees = 25,
                    IsConfirmed = false
                },
                 new EventModel
                {
                    Name = "Nothing's event",
                    Date = new DateTime(2024, 10, 10),
                    Location = "Venice",
                    Attendees = 10,
                    IsConfirmed = true
                },
                new EventModel
                {
                    Name = "Team Building",
                    Date = new DateTime(2024, 10, 27),
                    Location = "Venice",
                    Attendees = 20,
                    IsConfirmed = false
                },
                new EventModel
                {
                    Name = "Group bonding",
                    Date = new DateTime(2024, 10, 29),
                    Location = "Fiesta carnival",
                    Attendees = 15,
                    IsConfirmed = false
                }
            };

            // Convert to JSON for FullCalendar
            CalendarEventsJson = JsonConvert.SerializeObject(UpcomingEvents);
        }
    }

    public class EventModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Attendees { get; set; }
        public bool IsConfirmed { get; set; }

        public string Description { get; set; }
    }
}
