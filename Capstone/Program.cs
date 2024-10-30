using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Capstone.Configurations;
using Stripe;
using Microsoft.EntityFrameworkCore;
using Capstone.Hubs;
using Capstone.Data;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Add SignalR


// Configure database connection using the connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    string? clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    string? clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
    {
        throw new ArgumentException("Google ClientId and ClientSecret must be provided.");
    }

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.Scope.Add(Google.Apis.Calendar.v3.CalendarService.Scope.Calendar);
    options.SaveTokens = true;
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
app.MapControllers();

// Map the SignalR hub
app.MapHub<StaffDashboardHub>("/StaffDashboardHub");

app.Run();
