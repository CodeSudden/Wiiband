using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Capstone.Pages.Admin
{
    public class EventModel : PageModel
    {
        public List<Event> UpcomingEvents { get; set; }

        public string CalendarEventsJson { get; set; }

        public void OnGet()
        {
            // Initialize the upcoming events
            UpcomingEvents = new List<Event>
            {
                 new Event
                {
                    Name = "Birthday",
                    Date = new DateTime(2024, 10, 17),
                    Location = "Fiesta Carnival",
                    Attendees = 12,
                    IsConfirmed = true
                },
                new Event
                {
                    Name = "Anica's Birthday",
                    Date = new DateTime(2024, 10, 22),
                    Location = "Fiesta Carnival",
                    Attendees = 25,
                    IsConfirmed = false
                },
                 new Event
                {
                    Name = "Nothing's Event",
                    Date = new DateTime(2024, 10, 10),
                    Location = "Venice",
                    Attendees = 10,
                    IsConfirmed = true
                },
                new Event
                {
                    Name = "Team Building",
                    Date = new DateTime(2024, 10, 27),
                    Location = "Venice",
                    Attendees = 20,
                    IsConfirmed = false
                },
                new Event
                {
                    Name = "Group Bonding",
                    Date = new DateTime(2024, 10, 29),
                    Location = "Fiesta Carnival",
                    Attendees = 15,
                    IsConfirmed = false
                }
            };

            // Convert to JSON for FullCalendar
            CalendarEventsJson = JsonConvert.SerializeObject(UpcomingEvents);
        }
    }

    public class Event
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Attendees { get; set; }
        public bool IsConfirmed { get; set; }

        public string Description { get; set; }
    }
}
