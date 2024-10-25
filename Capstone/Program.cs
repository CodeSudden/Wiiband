using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Capstone.Configurations;
using Stripe;
using Google;
using Microsoft.EntityFrameworkCore;
using Capstone.Hubs;
using Capstone.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure database connection using the connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WiijumpDatabase")));


// Add Stripe configuration and load it from appsettings.json or environment variables
var stripeSettings = builder.Configuration.GetSection("Stripe");
builder.Services.Configure<StripeSettings>(stripeSettings);

// Add SignalR service
builder.Services.AddSignalR();

// Initialize Stripe with the secret key from the configuration
StripeConfiguration.ApiKey = stripeSettings.GetValue<string>("SecretKey");

// Add Google authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "902330764120-bpqt26o85gt02rk6r2dj9k7c0rqnin37.apps.googleusercontent.com";  // Use the client_id from the JSON
    options.ClientSecret = "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";  // Use the client_secret from the JSON
    options.Scope.Add(Google.Apis.Calendar.v3.CalendarService.Scope.Calendar);  // Add the Google Calendar scope
    options.SaveTokens = true;  // This ensures the access tokens are saved and accessible for API requests
});

// Register IHttpContextAccessor to access tokens
builder.Services.AddHttpContextAccessor();

// Register GoogleCalendarService (to handle Google Calendar operations)
builder.Services.AddScoped<GoogleCalendarService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add the authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Map the SignalR hub
app.MapHub<StaffDashboardHub>("/StaffDashboardHub");


app.Run();
