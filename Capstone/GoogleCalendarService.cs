using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class GoogleCalendarService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GoogleCalendarService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // This method retrieves the user's Google Calendar service using the saved access token
    public async Task<CalendarService> GetCalendarService()
    {
        // Get the access token saved during the user's authentication
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

        // Create Google credentials using the access token
        var credential = GoogleCredential.FromAccessToken(accessToken);

        // Create and return a CalendarService object
        return new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Capstone-Project"  // Replace with your desired application name
        });
    }

    // This method creates an event in Google Calendar
    public async Task CreateEvent(string summary, DateTime start, DateTime end, string location, string description)
    {
        var service = await GetCalendarService();

        Event newEvent = new Event()
        {
            Summary = summary,
            Location = location,
            Description = description,
            Start = new EventDateTime()
            {
                DateTime = start,
                TimeZone = "UTC"
            },
            End = new EventDateTime()
            {
                DateTime = end,
                TimeZone = "UTC"
            }
        };

        // Insert the event into the user's primary calendar
        EventsResource.InsertRequest request = service.Events.Insert(newEvent, "primary");
        await request.ExecuteAsync();
    }
}
